using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using AC_Physics;

//Blackboard system for Agents to access information
public class AIManager : MonoBehaviour
{
    public static AIManager aiManager;

    public Player player;
    public Agent[] agents;
    public List<Node> navGraph;

    //Store previously calculated paths, if same path is used again it can be quickly accessed through this dictionary
    public Dictionary<NodePair, List<Node>> calculatedPaths = new Dictionary<NodePair, List<Node>>();

    private void Awake()
    {
        if (aiManager == null)
            aiManager = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool PathExists(NodePair nodePair)
    {
        return calculatedPaths.ContainsKey(nodePair);
    }
   
    void OnDrawGizmos()
    {
        //Draws nav graph in unity editor only in scene view
        foreach (Node node in navGraph)
        {
            Vector3 center = node.transform.position;

            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(center, new Vector3(50, 50, 50));

            for (int i = 0; i < node.edges.Count; i++)
            {
                Gizmos.color = Color.red;
                GameObject connectedObject = node.edges[i].nextNode.gameObject;
                if (connectedObject)
                {
                    Gizmos.DrawLine(center, connectedObject.transform.position);
                }
                else
                {
                    Gizmos.DrawLine(center, Vector3.zero);
                }
            }
        }
    }
    //TODO: Generate node graph automatically based on min max positions and a distribution 

    ////Generate a grid of nodes
    //public void GenerateGridNavGraph(Vec2 min, Vec2 max, float distribution)
    //{
    //    float xInc = max.x - min.x / distribution;
    //    float yInc = max.y - min.y / distribution;

    //    float x = min.x;
    //    float y = min.y;

    //    while (x <= max.x)
    //    {
    //        while (y <= max.y)
    //        {

    //        }
    //    }
    //}

    ////Remove Nodes within objects
    //public void CleanNavGraph()
    //{

    //}

    //public void InitialiseAgentPath()
    //{
    //}
}
