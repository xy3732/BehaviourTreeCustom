using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;

    // 오브젝트의 값을 저장
    public Container container;
    void Awake()
    {
        // 기본적인 값을 설정. [rigidbody, transform 등] 저장.
        container = CreateBehaviourTreeContext();

        // 실행되면 트리 복사 해서 사용.
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
