using System;
using UnityEngine;

namespace SuperTrashBoy.BehaviorTrees
{
    public class Wait : Node
    {
        float timeToWait;
        float timer = 0f;

        public Wait(string n, float waitTime)
        {
            name = n;
            timeToWait = waitTime;
        }

        public override Status Process()
        {
            timer += Time.deltaTime;
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
