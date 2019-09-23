using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : RootNode
{
    override public NodeState ProcessNodes()
    {
        return NodeState.FAILURE;
    }
}
