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
    [RequireComponent(typeof(Animator))]
	public class AnimationEventsRewind : MonoBehaviour
	{
        public delegate void TriggerDelegate(string ID);
        public event TriggerDelegate OnTrigger;

		public virtual void Trigger(string ID)
        {
            if (OnTrigger != null) OnTrigger(ID);
        }
	}
}