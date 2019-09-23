using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AC_Physics;

namespace AI
{
    public class Agent: MonoBehaviour
    {
        //bezier curve variables
        private float time = 0f;
        private float bTime = 0f;
        private Vec2 p1 = Vec2.Zero;
        private Vec2 p2 = Vec2.Zero;
        private Vec2 p3 = Vec2.Zero;
        private Vec2 agentDirection = Vec2.Zero;

        [SerializeField] private Player aiPlayer;
        [SerializeField] private AStar agentPathfinding;

        [SerializeField] private List<Node> availableNodes;
        private List<Node> currentPath = new List<Node>();
        private Node currentNode;
        private NodePair nodePair;

        public List<Node> AvailableNodes
        {
            get { return availableNodes; }
        }
        
        public void Start()
        {
            //Currently uses all nodes for pathfinding, needs to be able to define own nodes to use
            availableNodes = AIManager.aiManager.navGraph;
            agentPathfinding.vertices = availableNodes;

            GetNextPath();
            if (currentPath.Count > 0)
                SetBezierCurvePoints(aiPlayer._RigidBody.Position, (currentPath[currentPath.Count - 1].position));

            time = Mathf.Max(0.5f, agentDirection.Magnitude() / 400f);
        }

        public void FixedUpdate()
        {
            //Custom bezier curve solution for path traversing
            //Bezier curve smooths path while providing directional velocity for node transitions
            //Can be seen in editor represented by coloured cubes and a line Red == p1, green == p2, blue == p3

            int pathSize = currentPath.Count;
            aiPlayer.horizontalInput = 0.0f;
            aiPlayer.verticalInput = 0.0f;

            //Calculate direction to next node based on AI position and node position
            //Check that the current node does not equal the final node in the path
            if (currentNode != currentPath[0])
            {
                //Calculate direction to next node
                Vec2 nextPos = currentNode.edges[currentNode.pathIndex].nextNode.position;
                agentDirection = (nextPos - aiPlayer._RigidBody.Position);

                if (bTime >= time)
                {                   
                    if (aiPlayer.CanJump)
                    {
                        //BezierCurve Smoothing for AI path
                        //Curve is created between current ai position and next node and will then apply a velocity along the curve direction
                        SetBezierCurvePoints(aiPlayer._RigidBody.Position, nextPos);

                        //Get a time based on distance and speed
                        time = Mathf.Max(0.5f, agentDirection.Magnitude() / 400f);
                        bTime = 0f;
                    }
                }

                if (agentDirection.Magnitude() < 50f)
                {
                    if (CalculateClosestNode(AIManager.aiManager.player._RigidBody.Position) != agentPathfinding.goalState)
                        GetNextPath();
                    else
                        currentNode = currentNode.edges[currentNode.pathIndex].nextNode;
                }
            }

            //check if current node is greater than jump height, calculate new path if needed
            if (currentNode.position.y - aiPlayer._RigidBody.Position.y > 400f)
            {
                GetNextPath();
                return;
            }

            if (currentNode != currentPath[0] && bTime <= time)
            {
                //Calculate normalised time for previous and current step
                float nTimePrev = bTime / time;
                bTime += Time.fixedDeltaTime;
                float nTimeCurrent = bTime / time;

                //Calculate previous and current position on curve to provide velocity direction
                float t = 1f - nTimePrev;
                Vec2 prevVel = ((t * t) * p1) + (2 * t * nTimePrev * p2) + ((nTimePrev * nTimePrev) * p3);

                t = 1f - nTimeCurrent;
                Vec2 currentVel = ((t * t) * p1) + (2 * t * nTimeCurrent * p2) + ((nTimeCurrent * nTimeCurrent) * p3);
                Vec2 appliedVel = currentVel - prevVel;

                //Allow gravity to affect AI normally
                if (appliedVel.y >= 0.0f)
                {
                    aiPlayer.CanJump = false;
                    aiPlayer._RigidBody.velocity.y = appliedVel.y;
                }
                else
                {
                    appliedVel.y = 0f;
                }

                aiPlayer._RigidBody.velocity.x = appliedVel.x;
            }
            else
            {
                //Once a path has been found let loose the AI to follow player controls
                agentDirection = (AIManager.aiManager.player._RigidBody.Position - aiPlayer._RigidBody.Position);
                if (agentDirection.Magnitude() > 500f)
                {
                    GetNextPath();
                }

                if (agentDirection.x > 0.0f)
                {
                    aiPlayer.horizontalInput = 1.0f;
                }
                else
                {
                    aiPlayer.horizontalInput = -1.0f;
                }

                if (agentDirection.y > 2.0f)
                {
                    if (aiPlayer.CanJump)
                    {
                        aiPlayer.verticalInput = 1.0f;
                    }
                }
                else
                {
                    aiPlayer.verticalInput = 0.0f;
                }
            }
        }

        //Used to set points in bezier curve calculation
        public void SetBezierCurvePoints(Vec2 point1, Vec2 point2)
        {
            //Get positions for bezier curve calculation
            p1 = point1;
            p3 = point2;
            p2.x = p1.x + (agentDirection.x * 0.5f);
            
            //y value is based on midpoint between p1 and p2 + the displacement y between the points
            if (currentNode.edges[currentNode.pathIndex].jump)
                p2.y = p1.y + Mathf.Abs(agentDirection.y * 2f);
            else
                p2.y = p1.y + agentDirection.y * 0.5f;

            p2.y = Mathf.Max(Mathf.Min(p1.y, p3.y), p2.y);
        }

        //Find a new path to player, if it exists already set the path to that instead
        public void GetNextPath()
        {
            //Create a node pair taking 2 nodes closest to player and AI
            //nodePair.A = CalculateClosestNode(aiPlayer._RigidBody.Position);
            //nodePair.B = CalculateClosestNode(AIManager.aiManager.player._RigidBody.Position);

            currentPath.Clear();

            agentPathfinding.initialState = CalculateClosestNode(aiPlayer._RigidBody.Position);
            agentPathfinding.goalState = CalculateClosestNode(AIManager.aiManager.player._RigidBody.Position);
            currentPath = agentPathfinding.CreateNewAStarPath();
            currentNode = currentPath[currentPath.Count - 1];

            Debug.Log("AI closest: " + CalculateClosestNode(aiPlayer._RigidBody.Position).name);
            Debug.Log("Player closest: " + CalculateClosestNode(AIManager.aiManager.player._RigidBody.Position).name);
            Debug.Log(currentNode.name);
        }

        //Loops through nodes to find node with shortest distance to passed position
        public Node CalculateClosestNode(Vec2 objectPos)
        {
            Node retVal = null;
            float distance = float.MaxValue;
            for (int i = 0; i < availableNodes.Count; i++)
            {
                float nodeDist = objectPos.DistanceSqr(availableNodes[i].position);
                if (distance > nodeDist)
                {
                    distance = nodeDist;
                    retVal = availableNodes[i];
                }
            }

            return retVal;
        }

        public void OnDrawGizmos()
        {
            //Draw bezier curves in editor
            Vec2 prevVel = p1;
            for (int i = 0; i < 20f; i++)
            {
                float t = (i / 20f);
                float oneMt = 1f - t;
                Vec2 currentVel = ((oneMt * oneMt) * p1) + (2 * oneMt * t * p2) + ((t * t) * p3);
                Gizmos.DrawLine(new Vector3(prevVel.x, prevVel.y, 0f), new Vector3(currentVel.x, currentVel.y, 0f));
                prevVel = new Vec2(currentVel);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(p1.x, p1.y, 0f), new Vector3(50f, 50f, 50f));
            Gizmos.DrawCube(new Vector3(p3.x, p3.y, 0f), new Vector3(50f, 50f, 50f));

            Gizmos.color = Color.blue;
            Gizmos.DrawCube(new Vector3(p2.x, p2.y, 0f), new Vector3(50f, 50f, 50f));
        }
    }
}
