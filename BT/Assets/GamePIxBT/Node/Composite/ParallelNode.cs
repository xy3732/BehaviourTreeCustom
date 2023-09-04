using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TheKiwiCoder
{
    public class ParallelNode : CompositeNode
    {
        List<State> childrenExcuteLeft = new List<State>();

        protected override void OnStart()
        {
            childrenExcuteLeft.Clear();

            children.ForEach((n) => 
            {
                childrenExcuteLeft.Add(State.Running);
            });
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            bool stillRunning = false;
            for (int i = 0; i < childrenExcuteLeft.Count(); ++i)
            {
                if (childrenExcuteLeft[i] == State.Running)
                {
                    var status = children[i].Update();

                    if (status == State.Failure)
                    {
                        RunChildrenAbort();
                        return State.Failure;
                    }

                    if (status == State.Running)
                    {
                        stillRunning = true;
                    }

                    childrenExcuteLeft[i] = status;
                }
            }

            return stillRunning ? State.Running : State.Success;
        }

        void RunChildrenAbort()
        {
            for (int i = 0; i < childrenExcuteLeft.Count(); ++i)
            {
                if (childrenExcuteLeft[i] == State.Running)
                {
                    children[i].Abort();
                }
            }
        }
    }
}