using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsStatus : MonoBehaviour
{
    [Range(1, 100)]
    public int fFont_Size;

    public Color color;

    float deltaTime = 0.0f;

    [Space()]
    public GameObject prefab;
    public int amount;
    private void Start()
    {
        Application.targetFrameRate = 122;

        for(int i= 0; i<amount; i++)
        {
            var obejcts = Instantiate(prefab);
        }    
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 0.02f);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = fFont_Size;
        style.normal.textColor = color;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format($"{msec:0.0} ms ({fps:0.} fps) : objects amount : {amount}");
        GUI.Label(rect, text, style);
    }
}