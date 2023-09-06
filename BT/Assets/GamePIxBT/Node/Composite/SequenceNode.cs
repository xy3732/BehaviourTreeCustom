using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.Profiling;
#endif

public class SequenceNode : CompositeNode
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
        Profiler.BeginSample("SequenceNode");
        // 현재 current번째에 있는 자식 노드를 가져오기.
        // 결과적으로 순서대로 실행이 안된 노드를 가져와서 실행 시킨다.
        var child = children[current];

        switch (child.Update())
        {
            case State.Running:
                return State.Running;
            case State.Failure:
                return State.Failure;
                // 만약 이미 실행한 노드 이면 current 증가.
            case State.Success:
                current++;
                break;
        }

        Profiler.EndSample();
        // 현재 실행중인 노드의 번째가 자식의 마지막 번째와 같은경우 성공 반환,
        // 아니라면 실행중으로 반환.
        return current == children.Count ? State.Success : State.Running;
    }
}
