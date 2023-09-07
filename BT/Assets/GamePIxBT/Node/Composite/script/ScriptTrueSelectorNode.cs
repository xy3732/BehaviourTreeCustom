using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptTrueSelectorNode : CompositeNode
{
    int current;
    protected override void OnStart()
    {
        container.isScriptTriger = false;
        current = 0;

        Debug.Log(children.Count);
    }

    protected override void OnStop()
    {
   
    }

    protected override State OnUpdate()
    {
        return State.Running;
    }
}
