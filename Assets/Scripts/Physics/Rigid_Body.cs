using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace AC_Physics
{
    public class Rigid_Body: MonoBehaviour
    {
        //Body type affects how an object will collide with another object
        //Dynamic bodies are affected by all physics including forces
        //Static bodies are affected by nothing and do not move
        //Kinematic bodies are controlled only through velocity and is not affected by forces
        public enum BodyType
        {
            DYNAMIC,
            STATIC,
            KINEMATIC
        }

        //Allows for objects to collide in separate layers
        public enum CollisionLayer
        {
            MAIN = 0,
            FOREGROUND,
            BACKGROUND
        }

        [SerializeField] private float gravityScale = 1f;
        [SerializeField] private float mass = 1f;
        [SerializeField] private float invMass = 0f;
        [SerializeField] private float drag = 0.2f;

        [SerializeField] private BodyType bodyType = BodyType.DYNAMIC;
        [SerializeField] private Base_Collider _collider;

        private Vec2 position = Vec2.Zero;
        private Vec2 centreOfMass = Vec2.Zero;
        private Vec2 force = Vec2.Zero;
        private Vec2 windVelocity = Vec2.Zero;

        public Vec2 velocity = Vec2.Zero;
        public Vec2 acceleration = Vec2.Zero;

        #region GetSets
        public Base_Collider Collider
        {
            get { return _collider; }
        }

        public Vec2 Position
        {
            get { return position; }
            set
            {
                //Set position for unity to use as well
                position = value;
                transform.position = new Vector3(position.x, position.y, 0f);
            }
        }

        public Vec2 Force
        {
            get { return force; }
        }

        public float Mass
        {
            get { return mass; }
        }

        public float InvMass
        {
            get { return invMass; }
        }
        #endregion

        public void OnEnable()
        {
            //Update position and add object back to world manager
            position = new Vec2(transform.position.x, transform.position.y);
            AddToWorldManager();
        }

        public void OnDisable()
        {
            //Remove object from world manager to stop collisions with it
            switch (bodyType)
            {
                case BodyType.DYNAMIC:
                    WorldManager.WMInstance.removeNextUpdate.Add(this);
                    break;
                case BodyType.KINEMATIC:
                    WorldManager.WMInstance.removeNextUpdate.Add(this);
                    break;
                case BodyType.STATIC:
                    WorldManager.WMInstance.removeNextUpdate.Add(this);
                    break;
            }
        }

        public void Start()
        {
            AddToWorldManager();

            //invMass can be used for quicker calulations
            invMass = 1f / mass;
            //Initialise variables
            acceleration = WorldManager.WMInstance.WorldGravity * gravityScale;
            position = new Vec2(transform.position.x, transform.position.y);
        }

        public void Update()
        {

        }

        public void FixedUpdate()
        {
            switch (bodyType)
            {
                case BodyType.DYNAMIC:
                    UpdateDynamicPosition();
                    break;
                case BodyType.KINEMATIC:
                    UpdateKinematicPosition();
                    break;
                case BodyType.STATIC:
                    break;
            }
        }

        public void AddToWorldManager()
        {
            //When a rigid body enter scene add it to the list of bodies in world manager
            switch (bodyType)
            {
                case BodyType.DYNAMIC:
                    if (!WorldManager.WMInstance.dynamicBodies.Contains(this))
                    WorldManager.WMInstance.dynamicBodies.Add(this);
                    break;
                case BodyType.KINEMATIC:
                    mass = 0f;
                    if (!WorldManager.WMInstance.kinematicBodies.Contains(this))
                        WorldManager.WMInstance.kinematicBodies.Add(this);
                    break;
                case BodyType.STATIC:
                    mass = 0f;
                    if (!WorldManager.WMInstance.staticBodies.Contains(this))
                        WorldManager.WMInstance.staticBodies.Add(this);
                    break;
            }
        }

        //Updates velocity and position based on acceleration and forces currently applied
        public void UpdateDynamicPosition()
        {
            //cache current deltaTime
            float deltaTime = Time.fixedDeltaTime;

            //Apply gravity scale of object to current world gravity, Allows for objects to fall differently given the same gravity
            Vec2 objectGravity = (WorldManager.WMInstance.WorldGravity * gravityScale);
            
            //Calculate acceleration based on current force and add that to the velocity
            acceleration = objectGravity + new Vec2(force.x * invMass, force.y * invMass);
            velocity += (acceleration * deltaTime);

            //Calculate crossSection for drag equation
            float crossSection = 0.0f;
            windVelocity = -velocity;

            //Check the type of collider being used
            if (_collider.Collider_Type == ColliderType.AABB)
            {
                crossSection = ((AABB_Collider)_collider).Width * Mathf.Abs(windVelocity.Normalized.DotProduct(Vec2.Right));
                crossSection += ((AABB_Collider)_collider).Height * Mathf.Abs(windVelocity.Normalized.DotProduct(Vec2.Up));
            }
            else if(_collider.Collider_Type == ColliderType.CIRCLE)
            {
                crossSection = ((Circle_Collider)_collider).Radius;
            }

            //Calculate drag and invert if force is facing negative direction
            Vec2 dragVelocity = Vec2.Zero;
            dragVelocity = 0.5f * WorldManager.wmInstance.AirDensity * drag * crossSection * (windVelocity * windVelocity);

            if (windVelocity.x < 0.0f)
                dragVelocity.x *= -1f;
            if (windVelocity.y < 0.0f)
                dragVelocity.y *= -1f;
            
            velocity += dragVelocity * deltaTime;

            //calculate displacement using acceleration
            float accX = 0.5f * acceleration.x * (deltaTime * deltaTime);
            float accY = 0.5f * acceleration.y * (deltaTime * deltaTime);

            //Update position using velocity and acceleration
            Position = new Vec2(position.x + velocity.x + accX, position.y + velocity.y + accY);
            force = Vec2.Zero;
        }

        public void UpdateKinematicPosition()
        {
            Position = new Vec2(position.x + velocity.x, position.y + velocity.y);
        }

        //Adds a force to the velocity calculation used next frame
        public void ApplyForceInstant(Vec2 _force)
        {
            force += _force;
        }

        public void ApplyForceInstant(float x, float y)
        {
            force.x += x;
            force.y += y;
        }
    }
}


////It DEAD
//public Dictionary<Base_Collider, CollisionInfo> currentCollidedWith = new Dictionary<Base_Collider, CollisionInfo>();
//public Dictionary<Base_Collider, CollisionInfo> lastCollidedWith = new Dictionary<Base_Collider, CollisionInfo>();
//public void UpdateCollisionState()
//{
//    foreach (var collider in currentCollidedWith)
//    {
//        if (lastCollidedWith.ContainsKey(collider.Key))
//        {
//            _collider.CollisionStay(collider.Key, collider.Value);
//        }
//        else
//        {
//            _collider.CollisionEnter(collider.Key, collider.Value);
//        }
//    }
//    foreach (var collider in lastCollidedWith)
//    {
//        if (!currentCollidedWith.ContainsKey(collider.Key))
//        {
//            _collider.CollisionExit(collider.Key, collider.Value);
//        }
//    }

//    lastCollidedWith = new Dictionary<Base_Collider, CollisionInfo>(currentCollidedWith);
//    currentCollidedWith.Clear();
//}