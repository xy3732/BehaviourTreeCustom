using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  [ DEBUG ] 해당 노드가 실행되는지 확인하는용.
/// </summary>
public class DebugLogNode : ActionNode
{
    [TextArea(4,2)]public string message;

    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        Debug.Log($"{message}");

        return State.Success;
    }

}
