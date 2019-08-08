using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game
{
	public class SetTowerSlotsContextOperation : Operation
	{
        [SerializeField]
        protected BaseTowerSlot target;
        public BaseTowerSlot Target { get { return target; } } 

        [SerializeField]
        protected BaseTowerSlotContextUI context;
        public BaseTowerSlotContextUI Context { get { return context; } } 

        public override void Execute()
        {
            context.Show(target);
        }
    }
}