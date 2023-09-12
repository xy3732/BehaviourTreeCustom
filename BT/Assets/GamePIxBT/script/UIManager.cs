using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    // 선택형 버튼 리스트
    [HideInInspector] public List<Button> selectButtonList = new List<Button>();

    // 대화 텍스트 출력
    [field: SerializeField] public TextMeshProUGUI textObject { get; set; }

    // UI 대화 버튼 선택창
    [field: Space(20)]
    [field: SerializeField] public GameObject buttonFrefab { get; set; }
    [field: SerializeField] public GameObject buttonSelectGroupObject { get; set; }
    public void Awake()
    {
        instance = this;

        textObject.text = "";
    }

    public void setText(string text)
    {
        textObject.text = text;
    }

    public void InitButtonSelection(int num, List<Node> children, Container container)
    {
        // 버튼 생성전에 리스트를 한번 초기화 시킨다.
        clearButtons();

        // 해당 노드의 자식의 수만큼 반복한다.
        for (int i = 0; i < num; i++)
        {
            // 버튼 생성
            var button = Instantiate(buttonFrefab);

            // 버튼에 해당 자식번호 저장
            button.AddComponent<selectionButtonNumber>();
            var selectNumber = button.GetComponent<selectionButtonNumber>().selectNumber;

            // 버튼 이름 지정
            button.name = children[i].description;

            // 버튼 부모 설정
            button.transform.SetParent(buttonSelectGroupObject.transform);

            // 버튼 클릭 이벤트 추가
            selectNumber = i;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                // 버튼 이벤트
                clickSelectBtn(selectNumber, container);
                clearButtons();
            });

            //리스트에 추가
            selectButtonList.Add(button.GetComponent<Button>());
        }
    }

    // 리스트 초기화
    public void clearButtons()
    {
        // 버튼 리스트가 null이 아닐시 버튼 리스트 초기화 및 삭제.
        if (selectButtonList != null)
        {
            for (int i = 0; i < selectButtonList.Count; i++)
            {
                Destroy(selectButtonList[i].gameObject);
            }

           selectButtonList.Clear();
        }
    }

    // 버튼 이벤트
    public void clickSelectBtn(int i, Container container)
    {
        container.buttonSelectNumber = i;
    }

}
