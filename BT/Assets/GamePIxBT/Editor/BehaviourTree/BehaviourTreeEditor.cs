using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;
#endif



public class BehaviourTreeEditor : EditorWindow
{
    BehaviourTreeView treeView;
    InspectorView inspectorView;
    IMGUIContainer behaviourTreeContainerView;
    Label treeViewLabel, behaviourTreeContainerLabel;
    ToolbarMenu toolbarMenu;
    ToolbarButton savetoolButton;
    VisualElement overlay;

    // Behaviour tree Container
    SerializedObject treeObject;
    SerializedProperty behaviourProperty;

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

        // Init 설정
        Setter(root);

        // Tree Editor의 Save를 누르면 해당 트리 저장.
        savetoolButton.clicked += () => SaveTree();

        // behaviour Tree Container 세팅
        BehaviourTreeContainerSet(root);

        // 툴바 메뉴 ["Asset"]버튼
        ToolBarMenuSet(root);

        treeView.OnNodeSelected = OnNodeSelectionChanged;

        OnSelectionChange();
    }
    private void BehaviourTreeContainerSet(VisualElement root)
    {
        behaviourTreeContainerView.onGUIHandler = () =>
        {
            // 트리오브젝트 가 없으면 BehaviourTreeEditor창을 열때 버그가 걸린다.
            // 해당 오브젝트가 없으면 실행 안되게 바꾼다.
            // 타겟 오브젝트도 없으면 에러 뜬다.
            if (treeObject != null && treeObject.targetObject != null)
            {
                // 선택된 트리가 없을시 overlay 띄우기
                overlay.style.visibility = Visibility.Hidden;

                treeObject.Update();
                EditorGUILayout.PropertyField(behaviourProperty);
                treeObject.ApplyModifiedProperties();
            }
        };
    }

    private void Setter(VisualElement root)
    {
        treeView = root.Q<BehaviourTreeView>();
        inspectorView = root.Q<InspectorView>();
        savetoolButton = root.Q<ToolbarButton>("SaveToolbar");
        overlay = root.Q<VisualElement>("Overlay");
        treeViewLabel = root.Q<Label>("treeView-Label");
        behaviourTreeContainerLabel = root.Q<Label>("BehaviourTreeContainer-Label");
        behaviourTreeContainerView = root.Q<IMGUIContainer>();
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
                // 현재 선택된 오브젝트가 있으면
                if (Selection.activeObject)
                {
                    // 스크립트를 선택하면 Error가 뜬다. 
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

            // TreeView 위에있는 라벨의 이름을 바꾼다.
            LabelNameChanger(tree);

            // 트리가 null이 아닐시만 behaviour container에 값 띄우기
            if (tree != null)
            {
                treeObject = new SerializedObject(tree);
                behaviourProperty = treeObject.FindProperty("btContainer");
            }
        };
    }
    private void ToolBarMenuSet(VisualElement root)
    {
        // 툴바 메뉴 ["Asset"]버튼
        toolbarMenu = root.Q<ToolbarMenu>();
        var behaviourTrees = LoadAssets<BehaviourTree>();
        behaviourTrees.ForEach((t) =>
        {
            toolbarMenu.menu.AppendAction($"{t.behaviourTreeType}/{t.name}", (a) =>
            {
                // 툴바 Asset에 있는거 클릭시 activeobject로 설정
                Selection.activeObject = t;
            });
        });
        toolbarMenu.menu.AppendSeparator();
    }

    // TreeView 위에있는 라벨의 이름을 바꾼다.
    private void LabelNameChanger(BehaviourTree tree)
    {
        if (tree == null) return;
        // TreeView 위에있는 라벨의 이름을 바꾼다.
        treeViewLabel.text = tree.name + " Tree View";
        behaviourTreeContainerLabel.text = tree.name + "'s container";
    }

    private List<T> LoadAssets<T>() where T : UnityEngine.Object
    {
        // 타입 : t: , "t:type" 특정 type 검색, 모든 에셋은 t:Object 으로 검색
        string[] assetIDs = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        // 찾은 모든 에셋을 임시로 저장 하는 리스트
        List<T> assets = new List<T>();

        foreach (var assetID in assetIDs)
        {
            //에셋 검색 후 로드
            string path = AssetDatabase.GUIDToAssetPath(assetID);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);

            assets.Add(asset);
        }

        return assets;
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