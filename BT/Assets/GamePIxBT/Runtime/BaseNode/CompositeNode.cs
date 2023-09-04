using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Composite는 다중으로 자식을 가질수 있다.
public abstract class CompositeNode : Node
{
    // 분기점 노드
    [HideInInspector] public List<Node> children = new List<Node>();

    // 해당 노드 복사해서 실행
    public override Node Clone()
    {
        CompositeNode node = Instantiate(this);
        node.children = children.ConvertAll((c) => c.Clone());

        return node;
    }
}
