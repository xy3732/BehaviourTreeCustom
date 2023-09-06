using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptEndNode : ActionNode
{
    public BehaviourTreeType nodeType = BehaviourTreeType.script;

    public bool alreadyRead = false;
    
    protected override void OnStart()
    {
        // 나중에 disable로 설정할것
        UIManager.instance.setText("");
    }

    protected override void OnStop()
    {

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
