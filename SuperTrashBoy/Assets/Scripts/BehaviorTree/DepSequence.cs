using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SuperTrashBoy.BehaviorTrees
{
    public class DepSequence : Node
    {
        BehaviorTree dependency;
        //NavMeshAgent agent;

        public DepSequence(string n, BehaviorTree d/*, NavMeshAgent a*/)
        {
            name = n;
            dependency = d;
            //agent = a;
        }

        public override Status Process()
        {
            Node.Status depStatus = dependency.Process();

            /////this may break something, comment in case
            while (depStatus == Status.RUNNING) depStatus = dependency.Process();

            //if (depStatus == Status.RUNNING) return Status.RUNNING;

            if (depStatus == Status.FAILURE)
            {
                //agent.ResetPath();
                foreach (Node n in children)
                {
                    n.Reset();
                }
                return Status.FAILURE;
            }   

            Status childStatus = children[currentChild].Process();
            if (childStatus == Status.RUNNING) return Status.RUNNING;
            if (childStatus == Status.FAILURE) return childStatus;

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