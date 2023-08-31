using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
