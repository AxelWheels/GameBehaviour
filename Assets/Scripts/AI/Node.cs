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
        public Edge(int _weight, Node _nextNode)
        {
            weight = _weight;
            nextNode = _nextNode;
        }

        public Node nextNode;
        public float weight;
    }

    public class Node: MonoBehaviour
    {
        public string name;
        public Node parent;
        public float currentWeight = 0f;
        public Vec2 position;
        [SerializeField] public List<Edge> edges = new List<Edge>();

        public Node(string _name, Vec2 _parent)
        {
            position = _parent;
            name = _name;
        }

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
