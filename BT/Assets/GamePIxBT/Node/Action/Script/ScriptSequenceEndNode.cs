using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptSequenceEndNode : ActionNode
{
    public bool alreadyRead = false;

    protected override void OnStart()
    {

        container.scriptSequence.selectNumber = container.scriptSequence.children.Count - 1;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (container.isScriptTriger)
        {
            alreadyRead = true;
        }

        return State.Success;
    }
}
