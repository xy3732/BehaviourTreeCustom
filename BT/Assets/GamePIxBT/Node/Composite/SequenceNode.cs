using System.Collections;
using System.Collections.Generic;


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
        UnityEngine.Profiling.Profiler.BeginSample("SequenceNode");
        // ���� current��°�� �ִ� �ڽ� ��带 ��������.
        // ��������� ������� ������ �ȵ� ��带 �����ͼ� ���� ��Ų��.
        var child = children[current];

        switch (child.Update())
        {
            case State.Running:
                return State.Running;
            case State.Failure:
                return State.Failure;
                // ���� �̹� ������ ��� �̸� current ����.
            case State.Success:
                current++;
                break;
        }

        UnityEngine.Profiling.Profiler.EndSample();
        // ���� �������� ����� ��°�� �ڽ��� ������ ��°�� ������� ���� ��ȯ,
        // �ƴ϶�� ���������� ��ȯ.
        return current == children.Count ? State.Success : State.Running;
    }
}
