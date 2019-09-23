using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using AI;

[CustomEditor(typeof(Node))]
[CanEditMultipleObjects]
class NodeToNodeConnector : Editor
{
    void OnInspectorGUI()
    {
        Node node = target as Node;
        node.name = node.gameObject.name;
    }

    void OnSceneGUI()
    {
        Node node = target as Node;

        Handles.color = Color.red;

        Vector3 center = node.transform.position;

        Handles.color = Color.yellow;
        Handles.DrawSolidArc(center, new Vector3(0, 0, 1), new Vector3(1, 0, 0), 360f, 20f);

        for (int i = 0; i < node.edges.Count; i++)
        {
            if (node.edges[i].nextNode == null)
                return;

            Handles.color = Color.red;
            GameObject connectedObject = node.edges[i].nextNode.gameObject;

            Handles.DrawLine(center, connectedObject.transform.position);

            Handles.color = Color.yellow;
            Handles.DrawSolidArc(connectedObject.transform.position, new Vector3(0, 0, 1), new Vector3(1, 0, 0), 360f, 20f);
        }   
    }
}
