using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [ Unity 3D ] Blackboard의 movePosition의 좌표까지 이동하는 ActionNode.
/// </summary>
public class MoveTo3Node : ActionNode
{
    // [todo] container 에서 설정된 값 받기.
    public float speed;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Vector3 dir;
        Quaternion lookTarget;

        dir = btContainer.moveToPosition - container.transform.position;
        lookTarget = Quaternion.LookRotation(dir);

        container.transform.position += dir.normalized * Time.deltaTime * speed;
        container.transform.rotation = Quaternion.Lerp(container.transform.rotation, lookTarget, 0.25f);

        if((container.transform.position - btContainer.moveToPosition).magnitude < 1f)
        {
            return State.Success;
        }

        return State.Running;
    }
}
