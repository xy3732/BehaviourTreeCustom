using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 하위 노드중에 State가 Failure인 노드를 실행하지 않고 다음 노드를 실행하는 CompositeNode
/// </summary>
public class TrueSelectorNode : CompositeNode
{
    int current;
    protected override void OnStart()
    {
        current = 0;
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        // 현재 current번째에 있는 자식 노드를 가져오기.
        // 결과적으로 순서대로 실행이 안된 노드를 가져와서 실행이된다.
        var child = children[current];

        switch (child.Update())
        {
            case State.Running:
                return State.Running;

            case State.Success:
                return State.Success;

                // 만약 실패한 노드 이면 패스.
            case State.Failure:
                current++;
                break;
        }

        // 현재 실행중인 노드의 번째가 자식의 마지막 번째와 같은경우 성공 반환,
        // 아니라면 실행중으로 반환.
        return current == children.Count ? State.Success : State.Running;
    }
}
