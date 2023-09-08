using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConfluenceNode : Node
{
    [HideInInspector] public Node child;

    // �ش� ��带 �����ؼ� ����.
    public override Node Clone()
    {
        ConfluenceNode node = Instantiate(this);
        node.child = child.Clone();

        return node;
    }
}
