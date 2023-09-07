using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptSequenceNode : CompositeNode
{
    protected override void OnStart()
    {
        // 컨테이너에 있는 버튼선택값을 -1로 초기화
        container.buttonSelectNumber = -1;

        // 자식 버튼들에게 순서대로 주소값 지정
        for(int i= 0; i< children.Count; i++) children[i].selectNode = i;

        // 버튼 생성
       UIManager.instance.InitButtonSelection(children.Count, children,container);
    }

    protected override void OnStop()
    {
   
    }

    protected override State OnUpdate()
    {

        // 버튼선택값이 없으면 실행 안되게 설정
        if (container.buttonSelectNumber != -1)
        {
            //버튼선택값이 -1이 아니라면
            for (int i = 0; i < children.Count; i++)
            {
                // 해당 버튼 주소값 이랑 컨테이너에 있는 버튼선택값이 같으면
                if (children[i].selectNode == container.buttonSelectNumber)
                {
                    // 해당 노드를 업데이트 한다
                    children[i].Update();

                    // 그 후에 버튼 오브젝트 삭제
                    UIManager.instance.clearButtons();
                }
            }
        }
        return State.Running;
    }
}
