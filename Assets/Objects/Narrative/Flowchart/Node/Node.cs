using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public abstract class Node : MonoBehaviour
	{
		public virtual void Execute()
        {
            
        }

        public event Action OnEnd;
        protected virtual void End()
        {
            OnEnd?.Invoke();
        }
	}
}