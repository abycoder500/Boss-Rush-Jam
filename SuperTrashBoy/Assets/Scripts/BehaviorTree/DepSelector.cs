using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperTrashBoy.BehaviorTrees
{
    public class DepSelector : Node
    {
        BehaviorTree dependency;

        public DepSelector(string n, BehaviorTree d)
        {
            name = n;
            dependency = d;
        }

        public override Status Process()
        {
            Node.Status depStatus = dependency.Process();
            while (depStatus == Status.RUNNING) depStatus = dependency.Process();
            //if (depStatus == Status.RUNNING) return Status.RUNNING;

            if (depStatus == Status.FAILURE)
            {
                foreach (Node n in children)
                {
                    n.Reset();
                }
                return Status.FAILURE;
            }

            Status childStatus = children[currentChild].Process();
            if (childStatus == Status.RUNNING) return Status.RUNNING;
            if (childStatus == Status.SUCCESS)
            {
                currentChild = 0;
                return Status.SUCCESS;
            }

            currentChild++;
            if (currentChild >= children.Count)
            {
                currentChild = 0;
                return Status.FAILURE;
            }

            return Status.RUNNING;
        }
    }
}