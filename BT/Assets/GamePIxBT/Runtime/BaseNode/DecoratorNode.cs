using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Decorator는 기본적으로 자식을 하나만 가질수 있다.
public abstract class DecoratorNode : Node
{
    public Node child;

    // 해당 노드를 복사해서 실행.
    public override Node Clone()
    {
        DecoratorNode node = Instantiate(this);

        node.child = child.Clone();

        return node;
    }
}
