using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;

#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEditor.Callbacks;
#endif



public class BehaviourTreeEditor : EditorWindow
{
    BehaviourTreeView treeView;
    InspectorView inspectorView;
    IMGUIContainer blackboardView;

    SerializedObject treeObject;
    SerializedProperty blackboardProperty;

    [MenuItem("GamePix's BT/Editor")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("GamePix's Behaviour Editor");
    }


    // ���� Ŭ������ BehaviourTree ����
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        if(Selection.activeObject is BehaviourTree)
        {
            OpenWindow();
            return true;
        }

        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/GamePIxBT/USS/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/GamePIxBT/USS/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviourTreeView>();
        inspectorView = root.Q<InspectorView>();
        blackboardView = root.Q<IMGUIContainer>();

        blackboardView.onGUIHandler = () =>
         {
             // Ʈ��������Ʈ �� ������ BehaviourTreeEditorâ�� ���� ���װ� �ɸ���.
             // �ش� ������Ʈ�� ������ ���� �ȵǰ� �ٲ۴�.
             if(treeObject != null)
             {
                 treeObject.Update();
                 EditorGUILayout.PropertyField(blackboardProperty);
                 treeObject.ApplyModifiedProperties();
             }
         };

        treeView.OnNodeSelected = OnNodeSelectionChanged;

        OnSelectionChange();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }


    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        // �÷��� ��� �߿����� Ŭ���� ������Ʈ�� Ʈ���並 Behaviour tree editor�� ���� �ִ�.
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;

            case PlayModeStateChange.ExitingEditMode:
                break;

            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;

            case PlayModeStateChange.ExitingPlayMode:
                break;
        }
    }

    private void OnSelectionChange()
    {
        EditorApplication.delayCall += () =>
        {
            BehaviourTree tree = Selection.activeObject as BehaviourTree;

            if (!tree)
            {
                if (Selection.activeObject)
                {
                    try
                    {
                        // ���� �������� ���ӿ�����Ʈ���� ������Ʈ�� �����´�
                        BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();

                        // ������Ʈ�� �����ϸ�
                        if (runner)
                        {
                            //Ʈ���� �ش� ������Ʈ�� Ʈ���� �ٲ۴�.
                            tree = runner.tree;
                        }
                    }
                    catch
                    {

                    }
                   
                }
            }

            // ����Ƽ���� ������ �������̶��
            if (Application.isPlaying)
            {
                // Behaviour tree editor â�� ����.
                if (tree)
                {
                    treeView.PopulateView(tree);
                }
            }
            else
            {
                if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                {
                    // treeView â ����
                    treeView.PopulateView(tree);
                }
            }

            if (tree != null)
            {
                treeObject = new SerializedObject(tree);
                blackboardProperty = treeObject.FindProperty("blackboard");
            }
        };
    }

    // ������ ��尡 �ٲ�� �ν����ͺ並 ������Ʈ
    void OnNodeSelectionChanged(NodeView node)
    {
        inspectorView.UpdateSelection(node);
    }

    // ���������� ������Ʈ
    private void OnInspectorUpdate()
    {
        treeView?.UpdateNodeStates();
    }
}