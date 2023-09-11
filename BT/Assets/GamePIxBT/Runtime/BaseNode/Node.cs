using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    public enum State
    {
        Running,
        Failure,
        Success
    }

    // 기본 State는 Running으로 설정
    [HideInInspector] public State state = State.Running;
    [HideInInspector] public bool started = false;
    public string guid;
    [HideInInspector] public Vector2 position;
    [HideInInspector] public BTContainer btContainer;
    [HideInInspector] public Container container;

    // 현재 노드의 설명문 작성 가능
    [TextArea(2,4)] public string description;
    public bool alreadySelected = false;
    // Script Only
    [HideInInspector] public int selectNode = 0;
    public State Update()
    {
        // 해당 노드가 처음 실행 될떄 Onstart를 실행
        if(!started)
        {
            OnStart();
            started = true;
        }

        // 해당 노드를 지속적으로 업데이트
        state = OnUpdate();

        // 해당 노드가 성공, 실패 하면 OnStop를 실행
        if(state == State.Failure || state == State.Success)
        {
            OnStop();
            started = false;
        }


        // 현재의 state를 반환한다.
        return state;
    }

    // 노드 강제 종료
    public void Abort()
    {
        BehaviourTree.Traverse(this, (n) => 
        {
            n.started = false;
            n.state = State.Running;
            n.OnStop();
        });
    }

    public virtual Node Clone()
    {
        return Instantiate(this);
    }

    protected abstract void OnStart();

    protected abstract State OnUpdate();
    protected abstract void OnStop();

}
