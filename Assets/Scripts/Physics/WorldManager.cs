//Collision detection algorithms and resolution found at 
//https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AC_Physics
{
    //World Manager handles physics interactions, this includes collision detection and impulse resolution
    [Serializable]
    class WorldManager : MonoBehaviour
    {
        //ToDo: Broad Phase collisions
        //      Wind force
        //      Trigger Colliders
        //      Convex Polygon Colliders
         
        public struct CollisionPair
        {
            Rigid_Body A;
            Rigid_Body B;

            public CollisionPair(Rigid_Body a, Rigid_Body b)
            {
                A = a;
                B = b;
            }
        }

        //Create a single instance of the world manager (Singleton Class)
        static public WorldManager wmInstance;
        [SerializeField] private Vec2 worldGravity;
        [SerializeField] private float airDensity;

        //Store all bodies that could be used in the world
        //Stored in separate lists to avoid checking collisions against objects that will never collide 
        //e.g. 2 static objects
        public List<Rigid_Body> dynamicBodies = new List<Rigid_Body>();
        public List<Rigid_Body> staticBodies = new List<Rigid_Body>();
        public List<Rigid_Body> kinematicBodies = new List<Rigid_Body>();

        public List<Rigid_Body> removeNextUpdate = new List<Rigid_Body>();

        public List<CollisionPair> broadPhaseCollisions = new List<CollisionPair>();

        #region GetSets
        public Vec2 WorldGravity
        {
            get { return worldGravity; }
        }

        public float AirDensity
        {
            get { return airDensity; }
        }

        static public WorldManager WMInstance
        {
            get { return wmInstance; }
        }
        #endregion

        public void Awake()
        {
            //Initialise static WorldManager
            if (wmInstance == null)
                wmInstance = this;
        }

        public void Start()
        {

        }

        public void Update()
        {

        }

        public void FixedUpdate()
        {
            for (int i = removeNextUpdate.Count - 1; i >= 0; i--)
            {
                dynamicBodies.Remove(removeNextUpdate[i]);
                staticBodies.Remove(removeNextUpdate[i]);
                kinematicBodies.Remove(removeNextUpdate[i]);
            }
            removeNextUpdate.Clear();

            int dCount = dynamicBodies.Count;
            //Loop through all interacting physics bodies and check against dynamic objects
            //Doesn't loop through unecessary checks this way
            for (int i = 0; i < dCount; i++)
            {
                for (int j = i; j < dCount; j++)
                {
                    if (dynamicBodies[i] != dynamicBodies[j])
                        DetectCollision(dynamicBodies[i], dynamicBodies[j]);
                }
                for (int j = 0, jc = staticBodies.Count; j < jc; j++)
                {
                    DetectCollision(dynamicBodies[i], staticBodies[j]);
                }
                for (int j = 0, jc = kinematicBodies.Count; j < jc; j++)
                {
                    DetectCollision(dynamicBodies[i], kinematicBodies[j]);
                }
            }

            //Call on collision events
            //for (int i = 0; i < dCount; i++)
            //{
            //    dynamicBodies[i].UpdateCollisionState();
            //}
            //for (int i = 0, ic = staticBodies.Count; i < ic; i++)
            //{
            //    staticBodies[i].UpdateCollisionState();
            //}
            //for (int i = 0, ic = kinematicBodies.Count; i < ic; i++)
            //{
            //    kinematicBodies[i].UpdateCollisionState();
            //}
        }

        //Checks if 2 colliders are intersecting and will run resolve if they are
        public void DetectCollision(Rigid_Body body1, Rigid_Body body2)
        {
            //1st collider in detection
            Base_Collider col1 = body1.Collider;
            //2nd collider in detection
            Base_Collider col2 = body2.Collider;

            CollisionInfo collision = new CollisionInfo();

            //Check what type of collision check needs to happen
            if (col1.Collider_Type == ColliderType.AABB && col2.Collider_Type == ColliderType.AABB)
            {
                collision = AABBvsAABB((AABB_Collider)col1, (AABB_Collider)col2);
            }
            else if (col1.Collider_Type == ColliderType.CIRCLE && col2.Collider_Type == ColliderType.CIRCLE)
            {
                collision = CIRCLEvsCIRCLE((Circle_Collider)col1, (Circle_Collider)col2);
            }
            else if (col1.Collider_Type == ColliderType.CIRCLE && col2.Collider_Type == ColliderType.AABB)
            {
                collision = CIRCLEvsAABB((Circle_Collider)col1, (AABB_Collider)col2, false);
            }
            else if (col1.Collider_Type == ColliderType.AABB && col2.Collider_Type == ColliderType.CIRCLE)
            {
                collision = CIRCLEvsAABB((Circle_Collider)col2, (AABB_Collider)col1, true);
            }

            if (collision.hit)
            {
                ResolveCollision(collision, body1, body2);
            }
        }

        public void BroadPhase()
        {

        }
        
        //This function handles the separating of collider, Resolution handles restitution, friction and positional correction
        public void ResolveCollision(CollisionInfo collisionInfo, Rigid_Body _collider, Rigid_Body _collided)
        {
            //impulse resolution, applies velocity to both rigid bodies involved in collision and positional correction is done for resting objects to stop
            //colliders sinking in to eachother. Mass is taken in to account any time velocity is changed.

            float invMassCollider = 0;
            float invMassCollided = 0;

            if (_collider.Mass != 0)
                invMassCollider = 1 / _collider.Mass;
            if (_collided.Mass != 0)
                invMassCollided = 1 / _collided.Mass;

            // Calculate relative velocity
            Vec2 rv = _collided.velocity - _collider.velocity;

            // Calculate relative velocity in terms of the normal direction
            float velAlongNormal = rv.DotProduct(collisionInfo.collisionNormal);

            // Do not resolve if velocities are separating
            if (velAlongNormal > 0)
                return;

            // Calculate restitution
            float e = Mathf.Min(_collider.Collider.P_Mat.restitution, _collided.Collider.P_Mat.restitution);

            // Calculate impulse scalar
            float j = -(1 + e) * velAlongNormal;
            j /= invMassCollider + invMassCollided;

            // Apply impulse
            Vec2 impulse = j * collisionInfo.collisionNormal;
            _collider.velocity -= invMassCollider * impulse;
            _collided.velocity += invMassCollided * impulse;

            // Re-calculate relative velocity after normal impulse is applied
            Vec2 impulseVel = _collided.velocity - _collider.velocity;

            // Solve for the tangent vector
            Vec2 tangent = impulseVel - impulseVel.DotProduct(collisionInfo.collisionNormal) * collisionInfo.collisionNormal;
            tangent.Normalize();

            // Solve for magnitude to apply along the friction vector
            float jt = -impulseVel.DotProduct(tangent);
            jt = jt / (invMassCollider + invMassCollided);

            //Get coefficient based on Material attached to colliders
            float mu = Physics_Material.CombineFrictions(_collider.Collider.P_Mat.staticFriction,
                                              _collided.Collider.P_Mat.staticFriction,
                                              _collider.Collider.P_Mat.frictionCombine);

            // Clamp magnitude of friction and create impulse vector
            Vec2 frictionImpulse;
            if (Mathf.Abs(jt) < j * mu)
                frictionImpulse = jt * tangent;
            else
            {
                frictionImpulse = -j * tangent * Physics_Material.CombineFrictions(_collider.Collider.P_Mat.dynamicFriction, 
                                                                                   _collided.Collider.P_Mat.dynamicFriction, 
                                                                                   _collider.Collider.P_Mat.frictionCombine);
            }

            // Apply friction impulse
            _collider.velocity -= invMassCollider * frictionImpulse;
            _collided.velocity += invMassCollided * frictionImpulse;

            //Correct for float precision error so object does not sink in to resting colliders
            const float percent = 0.8f; // usually 20% to 80%
            Vec2 correction = Vec2.Zero;
            correction.x = ( Mathf.Abs(collisionInfo.depth.x) / (invMassCollider + invMassCollided) ) * percent * collisionInfo.collisionNormal.x;
            correction.y = ( Mathf.Abs(collisionInfo.depth.y) / (invMassCollider + invMassCollided) ) * percent * collisionInfo.collisionNormal.y;
            //Debug.Log("correction: " + _collider.gameObject.name + " " + correction.x + " " + correction.y);
            _collider.Position -= invMassCollider * correction;
            _collided.Position += invMassCollided * correction;

            //Trigger on collision enter delegate. Default is an empty function
            _collider.Collider.CollisionEnter(_collided.Collider, collisionInfo);
            collisionInfo.collisionNormal *= -1f;
            _collided.Collider.CollisionEnter(_collider.Collider, collisionInfo);
        }

        private CollisionInfo CIRCLEvsAABB(Circle_Collider lhs, AABB_Collider rhs, bool invertNormal)
        {
            // Vector from A to B
            Vector3 n = lhs.transform.position - rhs.transform.position;

            // Closest point on A to center of B
            Vector3 closest = n;

            // Calculate half extents along each axis
            float x_extent = rhs.Width * 0.5f;
            float y_extent = rhs.Height * 0.5f;

            // Clamp point to edges of the AABB
            closest.x = Mathf.Clamp(closest.x, -x_extent, x_extent);
            closest.y = Mathf.Clamp(closest.y, -y_extent, y_extent);
            
            bool inside = false;

            // Circle is inside the AABB, so we need to clamp the circle's center
            // to the closest edge
            if (n == closest)
            {
                inside = true;

                // Find closest axis
                if (Mathf.Abs(n.x) > Mathf.Abs(n.y))
                {
                    // Clamp to closest extent
                    if (closest.x > 0)
                        closest.x = x_extent;
                    else
                        closest.x = -x_extent;
                }

                // y axis is shorter
                else
                {
                    // Clamp to closest extent
                    if (closest.y > 0)
                        closest.y = y_extent;
                    else
                        closest.y = -y_extent;
                }
            }

            Vector3 normal = n - closest;
            float d = normal.sqrMagnitude;
            float r = lhs.Radius;
            float penetration = 0f;

            // Early out if the radius is shorter than distance to closest point and
            // Circle not inside the AABB
            if (d > r * r && !inside)
                return new CollisionInfo();

            // Avoided sqrt until needed
            d = Mathf.Sqrt(d);

            // Collision normal needs to be flipped to point outside if circle was
            // inside the AABB
            if (inside)
            {
                normal = invertNormal ? -normal : normal;
            }
            else
            {
                normal = invertNormal ? normal : -normal;
            }

            penetration = r - d;
            Vec2 depth = new Vec2(normal.x, normal.y).Normalize();
            
            return new CollisionInfo(true, depth * penetration, depth);
        }

        private CollisionInfo CIRCLEvsCIRCLE(Circle_Collider lhs, Circle_Collider rhs)
        {
            Vec2 depth = Vec2.Zero;
            Vec2 vecDistConvert = Vec2.Zero;
            Vector3 vecDistance = lhs.transform.position - rhs.transform.position;
            float radius = lhs.Radius + rhs.Radius;

            if (radius * radius < vecDistance.sqrMagnitude)
                return new CollisionInfo();

            float distance = vecDistance.magnitude;
            float penetration = 0f;
            if (distance != 0f)
            {
                penetration = radius - distance;
                vecDistance.Normalize();
                vecDistConvert = new Vec2(-vecDistance.x, -vecDistance.y);
            }
            else
            {
                // Choose random (but consistent) values
                penetration = radius;
                vecDistConvert = new Vec2(1f, 0f);
            }
            
            depth = vecDistConvert * penetration;

            return new CollisionInfo(true, depth, vecDistConvert);
        }

        private CollisionInfo AABBvsAABB(AABB_Collider lhs, AABB_Collider rhs)
        {
            Vec2 lhsPos = new Vec2(lhs.transform.position.x, lhs.transform.position.y) + lhs.LocalOffset;
            Vec2 rhsPos = new Vec2(rhs.transform.position.x, rhs.transform.position.y) + rhs.LocalOffset;

            Vec2 lhsBottomLeft = lhs.BottomLeft + lhsPos;
            Vec2 lhsTopRight = lhs.TopRight + lhsPos;
            Vec2 rhsBottomLeft = rhs.BottomLeft + rhsPos;
            Vec2 rhsTopRight = rhs.TopRight + rhsPos;

            if (lhsBottomLeft.x < rhsTopRight.x &&
                lhsTopRight.x > rhsBottomLeft.x &&
                lhsTopRight.y > rhsBottomLeft.y &&
                lhsBottomLeft.y < rhsTopRight.y)
            {
                Vec2 vMult = Vec2.One;
                Vec2 depth = Vec2.Zero;
                Vec2 colNorm = Vec2.Zero;

                if (lhsPos.x > rhsPos.x)
                {
                    colNorm.x = -1f;
                    depth.x = rhsTopRight.x - lhsBottomLeft.x;
                }
                else
                {
                    colNorm.x = 1f;
                    depth.x = rhsBottomLeft.x - lhsTopRight.x;
                }
                if (lhsPos.y > rhsPos.y)
                {
                    colNorm.y = -1f;
                    depth.y = rhsTopRight.y - lhsBottomLeft.y;
                }
                else
                {
                    colNorm.y = 1f;
                    depth.y = rhsBottomLeft.y - lhsTopRight.y;
                }

                if (Mathf.Abs(depth.x) > Mathf.Abs(depth.y))
                {
                    colNorm.x = 0f;
                    depth.x = 0f;
                    vMult.y = 0f;
                }
                else
                {
                    colNorm.y = 0f;
                    depth.y = 0f;
                    vMult.x = 0f;
                }
                return new CollisionInfo(true, depth, colNorm);
            }

            return new CollisionInfo();
        }

        public bool PointInRectangle(Vec2 point, float left, float right, float top, float bottom)
        {
            if (point.x > left && point.x < right &&
                point.y < top && point.y > bottom)
            {
                return true;
            }
            return false;
        }

        public bool PointInCircle(Vec2 point, Vec2 circlePos, float radius)
        {
            float dist = circlePos.Distance(point);
            return dist <= radius;
        }

        //Creates an explosion force in world at position of body
        public void CreateExplosion(float explosionForce, Rigid_Body source)
        {
            Vec2 force = Vec2.Zero;
            Vec2 acceleration = Vec2.Zero;
            Vec2 direction = Vec2.Zero;

            Debug.Log(dynamicBodies.Count);
            for (int i = 0; i < dynamicBodies.Count; i++)
            {
                Debug.Log(dynamicBodies[i].gameObject.name);
                Debug.Log(source.gameObject.name);

                if (dynamicBodies[i].gameObject.name != source.gameObject.name)
                {
                    direction = dynamicBodies[i].Position - source.Position;

                    if (direction.Magnitude() < explosionForce)
                    {
                        force = explosionForce * direction.Normalized * ((explosionForce - direction.Magnitude()) / explosionForce);
                        dynamicBodies[i].ApplyForceInstant(force);
                    }
                }
            }
        }
    }
}