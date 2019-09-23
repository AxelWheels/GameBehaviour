using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AC_Physics;
using UnityEngine;

namespace AI
{
    public class AStar : MonoBehaviour
    {
        public List<Node> vertices = new List<Node>();
        public Node initialState;
        public Node goalState;
        public float currentWeight = 0;

        private List<Node> agentPath = new List<Node>();
        private Dictionary<string, Node> OpenList = new Dictionary<string, Node>();
        private Dictionary<string, Node> ClosedList = new Dictionary<string, Node>();
        private Node currentNode;

        public List<Node> CreateNewAStarPath()
        {
            InitialiseNodes();

            //Setting current node allows Astar to start opening nodes
            currentNode = initialState;

            //Empty all current lists to start fresh
            OpenList.Clear();
            ClosedList.Clear();
            agentPath.Clear();

            int depth = 0;
            while (CreateAStarPath())
            {
                depth++;

                //If there is no solution edges are not connected properly
                if (depth > 50)
                {
                    Debug.LogError("CHECK NODE EDGES ARE CONNECTED PROPERLY");
                }
            }
            //Debug.Log("Astar Depth: " + depth);
            return agentPath;
        }

        public bool CreateAStarPath()
        {
            if (ClosedList.ContainsKey(goalState.name))
            {
                //Debug.Log("Goal State Found " + ClosedList.Count);
                //foreach (KeyValuePair<string, Node> node in ClosedList)
                //{
                //    Debug.Log(node.Key + " " + node.Value.currentWeight);
                //}

                Node returnPath = ClosedList[goalState.name];

                do
                {
                    //Create 2 way trail to follow, allows for access to lots of information of AI path
                    if (returnPath.parent != null)
                    {
                        for (int i = 0; i < returnPath.parent.edges.Count; i++)
                        {
                            if (returnPath == returnPath.parent.edges[i].nextNode)
                            {
                                returnPath.parent.pathIndex = i;
                            }
                        }
                    }
                    
                    agentPath.Add(returnPath);
                    returnPath = returnPath.parent;
                    if (agentPath.Contains(returnPath))
                    {
                        Debug.Log("Return path in list " + returnPath.name);
                        break;
                    }
                } while (returnPath != null);
                OpenList.Clear();
                
                return false;
            }

            //Create temporary values for finding shortest distance to next node
            double tempWeight = Mathf.Infinity;
            Node tempLocation = currentNode;

            //Look for smallest distance to travel and set a temporary variable to use for Opening nodes
            foreach (KeyValuePair<string, Node> entry in OpenList)
            {
                float hv = CalculateHeuristicDistance(entry.Value);
                if (hv < tempWeight)
                {
                    tempWeight = hv;
                    tempLocation = entry.Value;
                }
            }

            bool closedHasLocation = ClosedList.ContainsKey(tempLocation.name);

            //OpenNode
            if (tempLocation.name != goalState.name)
            {
                //If closed list contains current node but weight is smaller open it, otherwise open node
                if (closedHasLocation)
                {
                    if (ClosedList[tempLocation.name].currentWeight > tempWeight)
                        OpenNode(tempLocation);
                }
                else
                    OpenNode(tempLocation);
            }
            
            //Add opened node to closed list
            if (closedHasLocation)
            {
                if (tempWeight < ClosedList[tempLocation.name].currentWeight)
                {
                    ClosedList.Remove(tempLocation.name);
                    ClosedList.Add(tempLocation.name, tempLocation);
                }
            }
            else
                ClosedList.Add(tempLocation.name, tempLocation);

            //Always remove node from open list once it has been opened
            OpenList.Remove(tempLocation.name);

            return true;
        }

        public void OpenNode(Node nodeToOpen)
        {
            //Debug.Log("Opening Node: " + nodeToOpen.name);
            //Debug.Log("Node Weight: " + nodeToOpen.currentWeight);
            bool isParent = false;

            //Loop through each edge and add it to the open list
            foreach (Edge edge in nodeToOpen.edges)
            {
                if (nodeToOpen.parent != null)
                    isParent = nodeToOpen.parent.name == edge.nextNode.name;

                //If the next node is the parent of the current node or in the closed list we do not want to open it
                if (!isParent && !ClosedList.ContainsKey(edge.nextNode.name))
                {
                    if (OpenList.ContainsKey(edge.nextNode.name))
                    {
                        //If already in open list replace if the weight is less than the current weight
                        if (edge.weight + OpenList[nodeToOpen.name].currentWeight < OpenList[edge.nextNode.name].currentWeight)
                        {
                            Node newNode = edge.nextNode;
                            newNode.currentWeight = edge.weight + OpenList[nodeToOpen.name].currentWeight;
                            newNode.parent = nodeToOpen;
                            OpenList.Remove(edge.nextNode.name);
                            OpenList.Add(edge.nextNode.name, newNode);
                        }
                    }
                    else
                    {
                        Node newNode = edge.nextNode;
                        newNode.currentWeight = edge.weight + nodeToOpen.currentWeight;
                        newNode.parent = nodeToOpen;
                        OpenList.Add(edge.nextNode.name, newNode);
                    }
                }
            }
        }
        
        //Reset all nodes to correct values before running astar pathing
        public void InitialiseNodes()
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i].position = new Vec2(vertices[i].transform.position.x, vertices[i].transform.position.y);
                vertices[i].parent = null;
                vertices[i].currentWeight = 0f;
            }
        }

        //Calculates a heuristic based on direct distance to final node
        public float CalculateHeuristicDistance(Node node)
        {
            Vec2 retVal = (node.position - goalState.position);
            //Debug.Log("Node heuristic: " + node.name + " " + retVal.Magnitude());
            return OpenList[node.name].currentWeight + retVal.Magnitude();
        }

        //Uncomment to see closed list in editor on play
        //public void OnGUI()
        //{
        //    GUIStyle style = new GUIStyle();
        //    style.fontSize = 40;
        //    style.normal.textColor = Color.white;

        //    foreach (KeyValuePair<string, Node> node in ClosedList)
        //    {
        //        GUILayout.Label(node.Key, style);
        //    }
        //}
    }
}

