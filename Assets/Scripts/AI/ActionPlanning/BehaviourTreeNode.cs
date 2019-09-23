using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeNode : MonoBehaviour
{
    public enum NodeState
    {
        SUCCESS = 0,
        FAILURE,
        RUNNING
    }

    public NodeState ExecuteBehaviour()
    {
        return NodeState.FAILURE;
    }
}
