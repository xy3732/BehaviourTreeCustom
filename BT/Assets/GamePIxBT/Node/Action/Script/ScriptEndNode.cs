using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptEndNode : ActionNode
{
    public BehaviourTreeType nodeType = BehaviourTreeType.script;

    public bool alreadyRead = false;
    
    protected override void OnStart()
    {
        // ���߿� disable�� �����Ұ�
        UIManager.instance.setText("");
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        // �ش� ��ũ��Ʈ ����� scriptTriger ����
        if (container.isScriptTriger)
        {
            alreadyRead = true;
            container.isScriptTriger = false;
        }

        // �ش� BehaviourTree ����
        return State.Success;
    }
}