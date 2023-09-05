using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomVector3Node : ActionNode
{
    public Vector3 min = Vector3.one * -10;
    public Vector3 max = Vector3.one * 10;

    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        btContainer.moveToPosition.x = Random.Range(min.x, max.x);
        btContainer.moveToPosition.y = Random.Range(min.y, max.y);
        btContainer.moveToPosition.z = Random.Range(min.z, max.z);

        return State.Success;
    }
}
