using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConfluenceNode : Node
{
    [HideInInspector] public Node child;

    // 해당 노드를 복사해서 실행.
    public override Node Clone()
    {
        ConfluenceNode node = Instantiate(this);
        node.child = child.Clone();

        return node;
    }
}
