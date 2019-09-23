using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AC_Physics;
using UnityEngine;

namespace AI
{
    [Serializable]
    public struct Edge
    {
        public Edge(int _weight, Node _nextNode, bool _jump)
        {
            weight = _weight;
            nextNode = _nextNode;
            jump = _jump;
        }

        public Node nextNode;
        public float weight;
        public bool jump;
    }

    //Node pairs can be used for a variety of thing
    //I mainly use them for dictionary access and setting InitialState and GoalState
    public struct NodePair
    {
        public Node A;
        public Node B;
    }

    public class Node: MonoBehaviour
    {
        public string name;
        public int pathIndex;
        public Node parent;
        public float currentWeight = 0f;
        public Vec2 position;
        [SerializeField] public List<Edge> edges = new List<Edge>();

        public void Awake()
        {
            position = new Vec2(transform.position.x, transform.position.y);
            name = gameObject.name;
        }

        public void Start()
        {
            InitialiseEdges();
        }

        public void InitialiseEdges()
        {
            for (int i = 0; i < edges.Count; i++)
            {
                Edge newEdge = edges[i];
                newEdge.weight = (edges[i].nextNode.position - position).Magnitude();
                edges[i] = newEdge;
            }
        }
    }
}
