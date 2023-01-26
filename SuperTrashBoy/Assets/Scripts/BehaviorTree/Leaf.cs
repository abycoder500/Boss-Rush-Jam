using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperTrashBoy.BehaviorTrees
{
    public class Leaf : Node
    {
        public delegate Status Tick();
        public Tick ProcessMethod;

        public delegate Status TickM(int val);
        public TickM ProcessMethodM;
        public int index;

        public Leaf() { }

        public Leaf(string n, Tick pm)
        {
            name = n;
            ProcessMethod = pm;
        }

        public Leaf(string n, TickM pm, int i)
        {
            name = n;
            ProcessMethodM = pm;
            index = i;
        }

        public Leaf(string n, Tick pm, int p)
        {
            name = n;
            ProcessMethod = pm;
            priority = p;
        }

        public override Status Process()
        {
            Node.Status s;

            if (ProcessMethod != null)
            {
                s = ProcessMethod();
            }

            else if (ProcessMethodM != null)
            {
                s = ProcessMethodM(index);
            }
            else
            {
                s = Status.FAILURE;
            }

            //Debug.Log(name + " " + s);
            return s;
        }
    }
}