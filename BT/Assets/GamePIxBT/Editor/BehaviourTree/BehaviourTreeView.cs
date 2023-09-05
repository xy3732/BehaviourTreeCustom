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
    // UI ToolKit - Library에 해당 스크립트로 만든 스타일 생성 [Project 내부에 있음]
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }

    public BehaviourTree tree;
    public Action<NodeView> OnNodeSelected;

    public BehaviourTreeView()
    {
        // 그리드 배경 추가
        Insert(0, new GridBackground());

        // 이벤트 추가 
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new DoubleClickSelection());        // selectionDragger 보다 위에 있을것
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        // uss 스타일 
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/GamePIxBT/USS/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);

        // ctrl + z, ctrl + y (Action)
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }

    public NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    // 트리뷰 변경 될때마다 실행 
    public void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OngGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OngGraphViewChanged;

        // Root Node 생성
        if(tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        // 노드 생성
        tree.nodes.ForEach((n) => CreateNodeView(n));

        // Edge 생성
        // Tree에 있는 Node리스트 내부에 있는 Edge 리스트를 이용하여 생성
        tree.nodes.ForEach((n) => 
        {
            var children = BehaviourTree.GetChildren(n);

            foreach (var c in children)
            {
                NodeView parentView = FindNodeView(n);
                NodeView childView = FindNodeView(c);

                Edge edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            }

        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => 
        endPort.direction != startPort.direction && 
        endPort.node != startPort.node).ToList();
    }

    // 그리그뷰 바뀌면 업데이트
    private GraphViewChange OngGraphViewChanged(GraphViewChange graphViewChange)
    {
        if(graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach((e) => 
            {
                // 노드 삭제시 리스트에서도 삭제되게 하기
                NodeView nodeView = e as NodeView;
                if(nodeView != null)
                {
                    tree.DeleteNode(nodeView.node);
                }

                // Edge 삭제시 노드 내부에 있는 리스트도 삭제되게 하기
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
            // 노드에 있는 리스트에 Edge 데이터 추가.
            graphViewChange.edgesToCreate.ForEach((e) =>
            {
                NodeView parentView = e.output.node as NodeView;
                NodeView nodeView = e.input.node as NodeView;

                tree.AddChild(parentView.node, nodeView.node);
            });
        }

        // 그레프에서 이동이 발동 되면 실행
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

    // 그리드뷰에 노드 생성
    void CreateNode(System.Type type, Vector2 pos)
    {
        Node node = tree.CreateNode(type);
        node.position = pos;

        CreateNodeView(node);
    }

    // Node를 Grid뷰에 생성
    void CreateNodeView(Node node)
    {
        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;

        AddElement(nodeView);
    }

    // 노드의 작동 State값을 업데이트 한다.
    public void UpdateNodeStates()
    {
        nodes.ForEach((n) => 
        {
            NodeView view = n as NodeView;
            view.UpdateState();
        });
    }

    // 마우스 오른쪽클릭 해서 뜨는 Context Menu
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        Vector2 mousePos = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);

        TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<Node>();

        foreach (Type type in types)
        {
            if (type.IsAbstract) continue;

            evt.menu.AppendAction($"{type.BaseType.Name}/{type.Name}", (a) =>
            {
                CreateNode(type, mousePos);
            });
        }

    }
}
