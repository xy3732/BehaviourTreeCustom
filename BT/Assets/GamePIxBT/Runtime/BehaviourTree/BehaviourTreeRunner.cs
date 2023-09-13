using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;

    // 오브젝트의 값을 저장
    public Container container = null;
    private void Awake()
    {
        bindTree();
    }

    private void Update()
    {
        // tree가 없으면 취소
        if (!tree) return;

        tree.Update();
    }

    private void bindTree()
    {
        // 컨테이너 생성
        // 기본적인 값을 설정. [rigidbody, transform 등] 저장.
        container = CreateBehaviourTreeContext();

        // 하위에 있는 스크립트 Init
        var component = gameObject?.GetComponentInChildren<BTIRange>();
        component?.OnRunnerAwake(container);

        Debug.Log(component);

        // 실행되면 트리 복사 해서 사용.
        tree = tree.Clone();
        tree.Bind(container);
    }


    // 추후 UImanager로 이동할 예정
    public void ScriptTrigerBtn()
    {
        container.isScriptTriger = true;
    }

    // 컨테이너 생성
    Container CreateBehaviourTreeContext()
    {
        // 이미 컨테이너를 가지고 있으면 리턴
        if (container != null) return container;
       
        // 없으면 생성
        return Container.CreateFromGameObject(gameObject);
    }
}
