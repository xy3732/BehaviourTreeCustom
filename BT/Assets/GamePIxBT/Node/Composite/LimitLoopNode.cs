using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitLoopNode : CompositeNode
{
    public int count;
   
    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        bool isStillLoop = false;

        for(int i= 0; i<count; ++i)
        {
            isStillLoop = true;
        }

        return isStillLoop ? State.Running : State.Success;
    }
}
