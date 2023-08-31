using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNode : Node
{
    public Node child;

    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        return child.Update();
    }

    // 해당 노드 복사해서 실행
    public override Node Clone()
    {
        RootNode node = Instantiate(this);
        node.child = child.Clone();

        return node;
    }
}
