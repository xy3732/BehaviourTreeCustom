using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 미완성 - 차후 Script Merge Node의 토대가 될것
/// 스크립트 합류지점
/// </summary>
public abstract class ConfluenceNode : Node
{
    public List<Node> parents = new List<Node>();
    [HideInInspector] public Node child;

    // 해당 노드를 복사해서 실행.
    public override Node Clone(Node input)
    {
        ConfluenceNode node = null;

        node = Instantiate(this);
        node.child = child.Clone();

        return node;
    }
}
