using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

public class EdgeView : UnityEditor.Experimental.GraphView.Edge
{
    public EdgeView(Edge edge)
    {
        UpdateState();
    }

    public void UpdateState()
    {
        AddToClassList("default-Edge");
    }

}
