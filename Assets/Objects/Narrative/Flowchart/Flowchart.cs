using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Flowchart : MonoBehaviour
    {
        public IList<Node> Nodes { get; protected set; }

        int index = 0;

        private void Awake()
        {
            Nodes = GetComponentsInChildren<Node>();
        }

        private void Start()
        {
            
        }

        public void Execute()
        {
            index = 0;

            if (Nodes.Count == 0)
                End();

            void Chain()
            {
                Nodes[index].OnEnd -= Chain;

                index++;

                if(index >= Nodes.Count)
                {
                    End();
                }
                else
                {
                    Nodes[index].OnEnd += Chain;
                    Nodes[index].Execute();
                }
            }

            Nodes[index].OnEnd += Chain;
            Nodes[index].Execute();
        }

        public event Action OnEnd;
        void End()
        {
            OnEnd?.Invoke();
        }
    }
}