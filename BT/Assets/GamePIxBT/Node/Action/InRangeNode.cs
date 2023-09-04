using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 범위내에 무언가가 있을때 실행되는 ActionNode
/// </summary>
public class InRangeNode : ActionNode
{
    protected override void OnStart()
    {
            
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if(container.isInrange == false) return State.Failure;
        else return State.Success;
    }
}
