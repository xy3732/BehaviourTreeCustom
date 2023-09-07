using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptSequenceNode : CompositeNode
{
    protected override void OnStart()
    {
        // �����̳ʿ� �ִ� ��ư���ð��� -1�� �ʱ�ȭ
        container.buttonSelectNumber = -1;

        // �ڽ� ��ư�鿡�� ������� �ּҰ� ����
        for(int i= 0; i< children.Count; i++) children[i].selectNode = i;

        // ��ư ����
       UIManager.instance.InitButtonSelection(children.Count, children,container);
    }

    protected override void OnStop()
    {
   
    }

    protected override State OnUpdate()
    {

        // ��ư���ð��� ������ ���� �ȵǰ� ����
        if (container.buttonSelectNumber != -1)
        {
            //��ư���ð��� -1�� �ƴ϶��
            for (int i = 0; i < children.Count; i++)
            {
                // �ش� ��ư �ּҰ� �̶� �����̳ʿ� �ִ� ��ư���ð��� ������
                if (children[i].selectNode == container.buttonSelectNumber)
                {
                    // �ش� ��带 ������Ʈ �Ѵ�
                    children[i].Update();

                    // �� �Ŀ� ��ư ������Ʈ ����
                    UIManager.instance.clearButtons();
                }
            }
        }
        return State.Running;
    }
}