using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;

    // ������Ʈ�� ���� ����
    public Container container;
    void Awake()
    {
        // �⺻���� ���� ����. [rigidbody, transform ��] ����.
        container = CreateBehaviourTreeContext();

        // ����Ǹ� Ʈ�� ���� �ؼ� ���.
        tree = tree.Clone();
        tree.Bind(container);
        
    }

    void Update()
    {
        if(tree)
        {
            tree.Update();
        }
    }

    public void ScriptTriger()
    {
        container.isScriptTriger = true;
    }


    Container CreateBehaviourTreeContext()
    {
        return Container.CreateFromGameObject(gameObject);
    }
}
