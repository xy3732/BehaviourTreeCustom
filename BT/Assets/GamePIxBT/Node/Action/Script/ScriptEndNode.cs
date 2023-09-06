using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptEndNode : ActionNode
{
    public bool alreadyRead = false;

    protected override void OnStart()
    {

        Debug.Log($"Start {this.name}");
    }

    protected override void OnStop()
    {
        Debug.Log($"Stop {this.name}");
    }

    protected override State OnUpdate()
    {
        // 해당 스크립트 종료시 scriptTriger 종료
        if (container.isScriptTriger)
        {
            alreadyRead = true;
            container.isScriptTriger = false;
        }

        // 해당 BehaviourTree 종료
        return State.Success;
    }
}
