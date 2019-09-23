using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : RootNode
{
    override public NodeState ProcessNodes()
    {
        return NodeState.FAILURE;
    }
}
