using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVector3Node : ActionNode
{
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        container.transform.position = new Vector3(blackboard.moveToPosition.x, blackboard.moveToPosition.y, blackboard.moveToPosition.z);

        return State.Success;
    }
}
