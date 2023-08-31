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

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;

    public Node node;

    public Port input;
    public Port output;
    public NodeView(Node node) : base("Assets/GamePIx's BT/USS/NodeView.uxml")
    {
        this.node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;

        // ����� ��ġ ����
        style.left = node.position.x;
        style.top = node.position.y;

        // ����� ��, �ƿ� ��Ʈ ����
        CreateInputPorts();
        CreateOutputPorts();

        // ��� ���� ����
        SetupClasses();

        // ��� �̹��� ����
        SetupNodeClasses();

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));
    }

    // ���� ����� �̸� ����
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
    }

    private void SetupNodeClasses()
    {
        if (node is ActionNode)
        {
            AddToClassList("ActionNode");
        }
        else if (node is CompositeNode)
        { 
            AddToClassList("CompositeNode");
        }
        else if (node is DecoratorNode)
        {
            AddToClassList("DecoratorNode");
        }
        else if (node is RootNode)
        {
            AddToClassList("RootNode");
        }
    }

    // ���� ����� ���� Ȯ��
    public void UpdateState()
    {
        // ������ �϶��� ���� ��� ����Ǽ� ������ �ִ� State���� �����Ѵ�.
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        // ���Ŀ� �ٽ� State���� �߰��Ѵ�.
        // [ ����Ƽ�� ���� ���� ���϶��� �۵� ]
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

    // Input��Ʈ ����
    private void CreateInputPorts()
    {
        if(node is ActionNode)
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
            // ��Ʈ ������ȭ �ʼ� �۾�
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }

    // Output��Ʈ ����
    private void CreateOutputPorts()
    {
        if (node is ActionNode)
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
            // ��Ʈ ������ȭ �ʼ� �۾�
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
    }

    // ����� ��ġ ����
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);

        // ctril + z, ctrl + y 
        Undo.RecordObject(node, "Behaviour Tree (SetPosition)");

        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;

        EditorUtility.SetDirty(node);
    }

    // ��带 �����ϸ� �۵�
    public override void OnSelected()
    {
        base.OnSelected();

        if(OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
    }

    // Ʈ������ ���� �������� ������ ������� ����Ʈ ���迭
    public void SortChildren()
    {
        CompositeNode composite = node as CompositeNode;

        if(composite)
        {
            composite.children.Sort(SortByHorizontalPosition);
        }
    }

    // ���ʿ������� ���������� ���� �迭
    private int SortByHorizontalPosition(Node left, Node right)
    {
        return left.position.x < right.position.x ? -1 : 1;
    }
}