using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

public class BehaviourTreeView : GraphView
{
    // UI ToolKit - Library�� �ش� ��ũ��Ʈ�� ���� ��Ÿ�� ���� [Project ���ο� ����]
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }

    public Action<NodeView> OnNodeSelected;

    BehaviourTree tree;

    public BehaviourTreeView()
    {
        // �׸��� ��� �߰�
        Insert(0, new GridBackground());

        // �̺�Ʈ �߰� 
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        // uss ��Ÿ�� 
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/GamePIx's BT/USS/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);

        // ctrl + z, ctrl + y (Action)
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }

    NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    // Ʈ���� ���� �ɶ����� ���� 
    public void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OngGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OngGraphViewChanged;

        // Root Node ����
        if(tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        // ��� ����
        tree.nodes.ForEach((n) => CreateNodeView(n));

        // Edge ����
        // Tree�� �ִ� Node����Ʈ ���ο� �ִ� Edge ����Ʈ�� �̿��Ͽ� ����
        tree.nodes.ForEach((n) => 
        {
            var children = tree.GetChildren(n);

            children.ForEach(c => 
            {
                NodeView parentView = FindNodeView(n);
                NodeView childView = FindNodeView(c);

                Edge edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => 
        endPort.direction != startPort.direction && 
        endPort.node != startPort.node).ToList();
    }

    // �׸��׺� �ٲ�� ������Ʈ
    private GraphViewChange OngGraphViewChanged(GraphViewChange graphViewChange)
    {
        if(graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach((e) => 
            {
                // ��� ������ ����Ʈ������ �����ǰ� �ϱ�
                NodeView nodeView = e as NodeView;
                if(nodeView != null)
                {
                    tree.DeleteNode(nodeView.node);
                }

                // Edge ������ ��� ���ο� �ִ� ����Ʈ�� �����ǰ� �ϱ�
                Edge edge = e as Edge;
                if (edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;

                    tree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        if(graphViewChange.edgesToCreate != null)
        {
            // ��忡 �ִ� ����Ʈ�� Edge ������ �߰�.
            graphViewChange.edgesToCreate.ForEach((e) =>
            {
                NodeView parentView = e.output.node as NodeView;
                NodeView nodeView = e.input.node as NodeView;

                tree.AddChild(parentView.node, nodeView.node);
            });
        }

        // �׷������� �̵��� �ߵ� �Ǹ� ����
        if(graphViewChange.movedElements != null)
        {
            nodes.ForEach((n) =>
            {
                NodeView view = n as NodeView;
                view.SortChildren();
            });
        }

        return graphViewChange;
    }

    // �׸���信 ��� ����
    void CreateNode(System.Type type)
    {
        Node node = tree.CreateNode(type);

        CreateNodeView(node);
    }

    // Node�� Grid�信 ����
    void CreateNodeView(Node node)
    {
        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;

        AddElement(nodeView);
    }

    // ����� �۵� State���� ������Ʈ �Ѵ�.
    public void UpdateNodeStates()
    {
        nodes.ForEach((n) => 
        {
            NodeView view = n as NodeView;
            view.UpdateState();
        });
    }

    // ���콺 ������Ŭ�� �ؼ� �ߴ� Context Menu
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();

            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) =>
                {
                    CreateNode(type);
                });
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();

            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) =>
                {
                    CreateNode(type);
                });
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();

            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) =>
                {
                    CreateNode(type);
                });
            }
        }
    }
}
