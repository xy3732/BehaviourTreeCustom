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
    ToolbarButton savetoolButton;
    VisualElement overlay;

    SerializedObject treeObject;
    SerializedProperty blackboardProperty;

    [MenuItem("GamePix's BT/Editor")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("GamePix's Behaviour Editor");
    }


    // 더블 클릭으로 BehaviourTree 열기
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
        savetoolButton = root.Q<ToolbarButton>("SaveToolbar");

        overlay = root.Q<VisualElement>("Overlay");

        // Tree Editor의 Save를 누르면 해당 트리 저장.
        savetoolButton.clicked += () => SaveTree();

        blackboardView = root.Q<IMGUIContainer>();
        blackboardView.onGUIHandler = () =>
         {
             // 트리오브젝트 가 없으면 BehaviourTreeEditor창을 열때 버그가 걸린다.
             // 해당 오브젝트가 없으면 실행 안되게 바꾼다.
             // 타겟 오브젝트도 없으면 에러 뜬다.
             if(treeObject != null && treeObject.targetObject != null)
             {
                 // 선택된 트리가 없을시 overlay 띄우기
                 overlay.style.visibility = Visibility.Hidden;

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
        SaveTree();
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        // 플레이 모드 중에서도 클릭한 오브젝트의 트리뷰를 Behaviour tree editor에 띄울수 있다.
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
                        // 현재 선택중인 게임오브젝트에서 컴포넌트를 가져온다
                        BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();

                        // 컴포넌트가 존재하면
                        if (runner)
                        {
                            //트리를 해당 오브젝트의 트리로 바꾼다.
                            tree = runner.tree;
                        }
                    }
                    catch
                    {

                    }
                   
                }
            }

            // 유니티에서 게임이 실행중이라면
            if (Application.isPlaying)
            {
                // Behaviour tree editor 창에 띄운다.
                if (tree)
                {
                    treeView.PopulateView(tree);
                }
            }
            else
            {
                if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                {
                    // treeView 창 열기
                    treeView.PopulateView(tree);
                }
            }

            if (tree != null)
            {
                treeObject = new SerializedObject(tree);
                blackboardProperty = treeObject.FindProperty("btContainer");
            }
        };
    }

    // 선택한 노드가 바뀌면 인스펙터뷰를 업데이트
    void OnNodeSelectionChanged(NodeView node)
    {
        inspectorView.UpdateSelection(node);
    }

    // 지속적으로 업데이트
    private void OnInspectorUpdate()
    {
        treeView?.UpdateNodeStates();
    }

    // 선택된 트리가 있고 유니티가 플레이 모드가 아닐시 저장 가능.
    private void SaveTree()
    {
        if (treeObject == null && !Application.isPlaying) return;

        Debug.Log("saved");
        AssetDatabase.SaveAssets();
    }
}