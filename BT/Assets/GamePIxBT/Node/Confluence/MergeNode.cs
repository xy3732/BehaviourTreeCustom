using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeNode : ConfluenceNode
{
    protected override void OnStart()
    {
        Debug.Log("test");
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        Debug.Log("update");
        child.Update();

        return State.Running;
    }
}
