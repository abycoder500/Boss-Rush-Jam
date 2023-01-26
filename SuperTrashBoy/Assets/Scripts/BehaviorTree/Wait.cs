using System;
using UnityEngine;

namespace SuperTrashBoy.BehaviorTrees
{
    public class Wait : Node
    {
        float timeToWait;
        float timer = 0f;
        float timeStep;

        public Wait(string n, float waitTime, float ts)
        {
            name = n;
            timeToWait = waitTime;
            timeStep = ts;
        }

        public override Status Process()
        {
            timer += timeStep;
            //Debug.Log(name + "Waited for " + timer + " s");
            if (timer >= timeToWait)
            {
                timer = 0f;
                return Status.SUCCESS;
            }
            return Status.RUNNING;
        }

        public override void Reset()
        {
            base.Reset();
            timer = Mathf.Infinity;
        }
    }
}
