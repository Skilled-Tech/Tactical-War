using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class ClosePageNode : Node
	{
        public Page target;

        protected virtual void Reset()
        {
            target = Dependancy.Get<Page>(gameObject, Dependancy.Scope.RecursiveToParents);
        }

        public override void Execute()
        {
            base.Execute();

            target.Close();

            End();
        }
    }
}