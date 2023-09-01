using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo3Node : ActionNode
{
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

        dir = blackboard.moveToPosition - container.transform.position;
        lookTarget = Quaternion.LookRotation(dir);

        container.transform.position += dir.normalized * Time.deltaTime * 2f;
        container.transform.rotation = Quaternion.Lerp(container.transform.rotation, lookTarget, 0.25f);

        if((container.transform.position - blackboard.moveToPosition).magnitude < 1f)
        {
            return State.Success;
        }

        return State.Running;
    }
}
