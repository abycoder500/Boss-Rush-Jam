using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperTrashBoy.BehaviorTrees
{
    public class Sequence : Node
    {
        public Sequence(string n)
        {
            name = n;
        }

        public override Status Process()
        {
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

            currentChild ++;
            if (currentChild >= children.Count) 
            {   
                currentChild = 0;
                return Status.SUCCESS;
            }

            return Status.RUNNING;
        }
    }
}
