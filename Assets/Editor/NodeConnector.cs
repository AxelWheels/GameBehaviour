using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AI;

[CustomEditor(typeof(AStar))]
[CanEditMultipleObjects]
class NodeConnector : Editor
{
    void OnSceneGUI()
    {
        AStar aStar = target as AStar;

        Handles.color = Color.red;

        foreach (Node node in aStar.vertices)
        {
            Vector3 center = node.transform.position;
            
            for (int i = 0; i < node.edges.Count; i++)
            {
                Handles.color = Color.red;
                GameObject connectedObject = node.edges[i].nextNode.gameObject;
                if (connectedObject)
                {
                    Handles.DrawLine(center, connectedObject.transform.position);
                }
                else
                {
                    Handles.DrawLine(center, Vector3.zero);
                }
            }
        }

        foreach (Node node in aStar.vertices)
        {
            if (aStar.ClosedList.ContainsKey(node.name))
            {
                Handles.color = Color.green;
            }
            else
            {
                Handles.color = Color.yellow;
            }
            Vector3 center = node.transform.position;
            Handles.DrawSolidArc(center, new Vector3(0, 0, 1), new Vector3(1, 0, 0), 360f, 20f);
        }

    }
}
