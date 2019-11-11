using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game
{
	public class EventNode : Node
	{
        [SerializeField]
        UnityEvent onExecute = new UnityEvent();
        public UnityEvent OnExecute => onExecute;

        public override void Execute()
        {
            base.Execute();

            OnExecute.Invoke();

            End();
        }
    }
}