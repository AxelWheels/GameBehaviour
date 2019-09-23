using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNode : BehaviourTreeNode
{
    [SerializeField]
    protected BehaviourTreeNode[] children;

    public BehaviourTreeNode[] Children
    {
        get { return children; }
    }

    virtual public NodeState ProcessNodes()
    {
        return NodeState.FAILURE;
    }
}
