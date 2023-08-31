using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;

    // ������Ʈ�� ���� ����
    Container container;
    void Start()
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

    Container CreateBehaviourTreeContext()
    {
        return Container.CreateFromGameObject(gameObject);
    }
}
