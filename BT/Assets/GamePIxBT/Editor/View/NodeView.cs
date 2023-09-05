using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

/// <summary>
/// BehaviourTreeEditor 에서 Node
/// </summary>
public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;

    public Node node;

    public Port input;
    public Port output;
    public NodeView(Node node) : base("Assets/GamePIxBT/USS/NodeView.uxml")
    {
        this.node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;

        // 노드의 위치 저장
        style.left = node.position.x;
        style.top = node.position.y;

        // 노드의 인, 아웃 포트 생성
        CreateInputPorts();
        CreateOutputPorts();

        // 노드 색상 설정
        SetupClasses();

        // 노드 이미지 설정
        SetupNodeClasses();

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));
    }

    // 개별 노드의 이름 설정
    private void SetupClasses()
    {
        if (node is ActionNode)
        {
            AddToClassList("action");
        }
        else if (node is CompositeNode)
        {
            AddToClassList("composite");
        }
        else if (node is DecoratorNode)
        {
            AddToClassList("decorator");
        }
        else if (node is RootNode)
        {
            AddToClassList("root");
        }
        else if(node is ConditionNode) //6B5CA5
        {
            AddToClassList("condition");
        }
    }

    private void SetupNodeClasses()
    {
        if (node is DebugLogNode)
        {
            AddToClassList("DebugLogNode");
        }
        else if (node is WaitNode)
        { 
            AddToClassList("WaitNode");
        }
        else if (node is SequenceNode)
        {
            AddToClassList("SequenceNode");
        }
        else if (node is RootNode)
        {
            AddToClassList("RootNode");
        }
        else if(node is RepeatNode)
        {
            AddToClassList("RepeatNode");
        }
        else if(node is TrueSelectorNode)
        {
            AddToClassList("TrueSelectorNode");
        }
        else if(node is MoveTo3Node)
        {
            AddToClassList("MoveToPosNode");
        }
        else if(node is InRangeNode)
        {
            AddToClassList("InRangeNode");
        }
        else if(node is RandomVector3Node)
        {
            AddToClassList("RandomNode");
        }
    }

    // 개별 노드의 실행 확인
    public void UpdateState()
    {
        // 실행중 일때는 값이 계속 변경되서 가지고 있던 State값을 삭제한다.
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        // 그후에 다시 State값을 추가한다.
        // [ 유니티가 게임 실행 중일때만 작동 ]
        if (Application.isPlaying)
        {
            switch (node.state)
            {
                case Node.State.Running:
                    if(node.started)
                    {
                        AddToClassList("running");
                    }
                    break;

                case Node.State.Failure:
                    AddToClassList("failure");
                    break;

                case Node.State.Success:
                    AddToClassList("success");
                    break;

            }
        }
    }

    // Input포트 생성
    private void CreateInputPorts()
    {
        if(node is ActionNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));    
        }
        else if (node is ConditionNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if(node is CompositeNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if(node is DecoratorNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if(node is RootNode)
        {
          
        }


        if (input!=null)
        {
            input.portName = "";
            // 포트 제정렬화 필수 작업
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }

    // Output포트 생성
    private void CreateOutputPorts()
    {
        if (node is ActionNode)
        {
           
        }
        else if(node is ConditionNode)
        {

        }
        else if (node is CompositeNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (node is DecoratorNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else if (node is RootNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }


        if (output != null)
        {
            output.portName = "";
            // 포트 제정렬화 필수 작업
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
    }

    // 노드의 위치 설정
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);

        // ctril + z, ctrl + y 
        Undo.RecordObject(node, "Behaviour Tree (SetPosition)");

        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;

        EditorUtility.SetDirty(node);
    }

    // 노드를 선택하면 작동
    public override void OnSelected()
    {
        base.OnSelected();

        if(OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }

    // 트리뷰의 왼쪽 에서부터 오른쪽 순서대로 리스트 제배열
    public void SortChildren()
    {
        CompositeNode composite = node as CompositeNode;

        if(composite)
        {
            composite.children.Sort(SortByHorizontalPosition);
        }
    }

    // 왼쪽에서부터 오른쪽으로 순서 배열
    private int SortByHorizontalPosition(Node left, Node right)
    {
        return left.position.x < right.position.x ? -1 : 1;
    }
}
