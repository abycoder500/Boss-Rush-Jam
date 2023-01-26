using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperTrashBoy.BehaviorTrees
{
    public class Loop : Node
    {
        BehaviorTree dependency;

        public Loop(string n, BehaviorTree d)
        {
            name = n;
            dependency = d;
        }

        public override Status Process()
        {
            if(dependency.Process() == Status.FAILURE)
            {
                return Status.SUCCESS;
            }
            Status childStatus = children[currentChild].Process();
            if (childStatus == Status.RUNNING) return Status.RUNNING;
            if (childStatus == Status.FAILURE)
            {
                currentChild = 0;
                foreach (Node n in children)
                {
                    n.Reset();
                }
                return Status.FAILURE;
            }

            currentChild++;
            if (currentChild >= children.Count)
            {
                currentChild = 0;
            }

            return Status.RUNNING;
        }
    }
}
