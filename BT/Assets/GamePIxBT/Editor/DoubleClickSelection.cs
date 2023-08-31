using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

// Node 더블 클릭 하면 하위 노드 까지 선택되게 설정
public class DoubleClickSelection : MouseManipulator    
{
    double time;
    double doubleClickDuration = 0.3f;

    public DoubleClickSelection()
    {
        time = EditorApplication.timeSinceStartup;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
    }

    protected override void UnregisterCallbacksFromTarget()
    {

        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
    }


    // 마우스 클릭시
    private void OnMouseDown(MouseDownEvent evt)
    {
        var graphview = target as BehaviourTreeView;

        // 그레프뷰가 없으면 반환
        if(graphview == null) return;

        double duration = EditorApplication.timeSinceStartup - time;
        // 더블클릭 설정 시간보다 짧으면 더블클릭 코드 싱행 
        if(duration < doubleClickDuration)
        {
            SelectChildren(evt);
        }

        time = EditorApplication.timeSinceStartup;
    }

    // 노드의 하위 노드까지 선택
    void SelectChildren(MouseDownEvent evt)
    {
        var graphView = target as BehaviourTreeView;

        // 그레프뷰가 없으면 반환
        if (graphView == null) return;

        if (!CanStopManipulation(evt)) return;

        NodeView clickedElement = evt.target as NodeView;

        if(clickedElement == null)
        {
            var ve = evt.target as VisualElement;
            clickedElement = ve.GetFirstAncestorOfType<NodeView>();

            if (clickedElement == null) return;
        }

        // 하위 노드 까지 선택하는 메소드
        BehaviourTree.Traverse(clickedElement.node, (n) => 
        {
            var view = graphView.FindNodeView(n);
            graphView.AddToSelection(view);
        });
    }
}
