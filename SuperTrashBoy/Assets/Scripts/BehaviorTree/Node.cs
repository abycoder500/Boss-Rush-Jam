using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperTrashBoy.BehaviorTrees
{
    public class Node
    {
        public enum Status { SUCCESS, RUNNING, FAILURE};

        public Status status;
        public List<Node> children = new List<Node>();
        public int currentChild = 0;
        public string name;
        public int priority;

        // creo tre constructor per il nodo, uno in cui specifico il nome, uno in cui specifico nome e priority, uno in cui il nodo non ha un nome
        public Node() { }

        public Node(string n)
        {
            name = n;
        }

        public virtual void Reset()
        {
            foreach (Node n in children)
            {
                n.Reset();
            }
            currentChild = 0;
        }

        public Node(string n, int p)
        {
            name = n;
            priority = p;
        }



        public virtual Status Process()
        {
            return children[currentChild].Process();
        }

        public void AddChild(Node n)
        {
            children.Add(n);
        }
    }
}
