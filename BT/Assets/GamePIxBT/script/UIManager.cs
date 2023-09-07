using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [field:SerializeField ] public TextMeshProUGUI textObject { get; set; }

    public void Awake()
    {
        instance = this;

        textObject.text = "";
    }

    public void setText(string text)
    {
        textObject.text = text;
    }
}
