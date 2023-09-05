using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;
    public Node.State treeState = Node.State.Running;

    public List<Node> nodes = new List<Node>();
    public BTContainer btContainer = new BTContainer();
    public Node.State Update()
    {
        if(rootNode.state == Node.State.Running)
        {
            treeState = rootNode.Update();
        }
        return treeState;
    }
    #region UnityEditor
#if UNITY_EDITOR
    // Node 생성
    public Node CreateNode(System.Type type)
    {
        // Node 생성 데이터 
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();

        //ctrl+z, ctrl+y
        Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
        // 리스트에 저장
        nodes.Add(node);

        if(Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }

        // 데이터 저장
        AssetDatabase.AddObjectToAsset(node, this);
        Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");
        AssetDatabase.SaveAssets();

        return node;
    }

    // Node 삭제시 List에서도 삭제
    public void DeleteNode(Node node)
    {
        //ctrl+z, ctrl+y
        Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");

        nodes.Remove(node);

        //AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);
        AssetDatabase.SaveAssets();
    }

    // 노드 끼리 부모자식 상속 관계를 만든다
    public void AddChild(Node parent, Node child)
    {
        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");
            rootNode.child = child;
            EditorUtility.SetDirty(rootNode);
        }

        DecoratorNode decorator = parent as DecoratorNode;
        if(decorator)
        {
            Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
            decorator.child = child;
            EditorUtility.SetDirty(decorator);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
            composite.children.Add(child);
            EditorUtility.SetDirty(composite);
        }
    }

    // 노드 끼리 만들어진 부모자식 상속 관계를 삭제한다.
    public void RemoveChild(Node parent, Node child)
    {
        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");
            rootNode.child = null;
            EditorUtility.SetDirty(rootNode);
        }

        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator)
        {
            Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
            decorator.child = null;
            EditorUtility.SetDirty(decorator);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
            composite.children.Remove(child);
            EditorUtility.SetDirty(composite);
        }
    }

    // 노드 별 자식값을 반환
    public static List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();

        RootNode rootNode = parent as RootNode;
        if (rootNode && rootNode.child != null)
        {
            children.Add(rootNode.child);
        }

        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator && decorator.child != null)
        {
            children.Add(decorator.child);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            return composite.children;
        }

        return children;
    }
#endif
    #endregion

    public static void Traverse(Node node, System.Action<Node> visiter)
    {
        if(node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }
    }

    // Clone으로 실행
    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone();

        tree.nodes = new List<Node>();
        Traverse(tree.rootNode, (n) => 
        {
            tree.nodes.Add(n);
        });

        return tree;
    }

    public void Bind(Container container)
    {
        Traverse(rootNode, (n) => 
        {
            n.container = container;
            n.btContainer = btContainer;
        });
    }
}
