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
        private List<Node> agentPath = new List<Node>();

        public Dictionary<string, Node> OpenList = new Dictionary<string, Node>();
        public Dictionary<string, Node> ClosedList = new Dictionary<string, Node>();

        public Node InitialState;
        public Node GoalState;
        private Node currentNode;

        public float currentWeight = 0;

        public void Update()
        {
        }

        public List<Node> CreateNewAStarPath()
        {
            InitialiseNodes();

            currentNode = InitialState;
            OpenList.Clear();
            ClosedList.Clear();
            agentPath.Clear();

            int depth = 0;
            while (CreateAStarPath())
            {
                depth++;
            }
            return agentPath;
        }

        public bool CreateAStarPath()
        {
            if (ClosedList.ContainsKey(GoalState.name))
            {
                //Debug.Log("Goal State Found " + ClosedList.Count);
                //foreach (KeyValuePair<string, Node> node in ClosedList)
                //{
                //    Debug.Log(node.Key + " " + node.Value.currentWeight);
                //}

                Node returnPath = ClosedList[GoalState.name];

                do
                {
                    agentPath.Add(returnPath);
                    returnPath = returnPath.parent;
                } while (returnPath != null);
                OpenList.Clear();
                
                return false;
            }

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

            //if (OpenList.ContainsKey(tempLocation.name))
            //    tempWeight = OpenList[tempLocation.name].currentWeight;

            bool closedHasLocation = ClosedList.ContainsKey(tempLocation.name);

            //OpenNode
            if (tempLocation.name != GoalState.name)
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
            

            //If we already have the node in the closed list check if the weight is smaller so we can replace it, otherwise add to closed list
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

            return OpenList.Count > 0;
        }

        public void OpenNode(Node nodeToOpen)
        {
            //Debug.Log("Opening Node: " + nodeToOpen.name);
            //Debug.Log("Node Weight: " + nodeToOpen.currentWeight);
            bool isParent = false;
            foreach (Edge edge in nodeToOpen.edges)
            {
                if (nodeToOpen.parent != null)
                    isParent = nodeToOpen.parent.name == edge.nextNode.name;

                if (!isParent)
                {
                    if (OpenList.ContainsKey(edge.nextNode.name))
                    {
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

        public void InitialiseNodes()
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i].parent = null;
                vertices[i].currentWeight = 0f;
            }
        }


        public float CalculateHeuristicDistance(Node node)
        {
            Vec2 retVal = (node.position - GoalState.position);
            return OpenList[node.name].currentWeight + retVal.Magnitude();
        }
    }
}
