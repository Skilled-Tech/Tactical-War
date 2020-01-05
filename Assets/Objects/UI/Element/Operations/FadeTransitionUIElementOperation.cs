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
	public class FadeTransitionUIElementOperation : Operation
	{
        [SerializeField]
        protected UIElement current;

        [SerializeField]
        protected UIElement target;

        [SerializeField]
        protected float duration = 0.2f;
        public float Duration { get { return duration; } }

        public Core Core => Core.Instance;

        public FaderUI Fader => Core.UI.Fader;

        protected virtual void Reset()
        {
            current = Dependancy.Get<UIElement>(gameObject, Dependancy.Scope.RecursiveToParents);
        }

        public override void Execute()
        {
            Core.SceneAcessor.StartCoroutine(Procedure());

            IEnumerator Procedure()
            {
                yield return Fader.To(1f, duration);

                if (current == null)
                    Debug.LogWarning("No current UI Element set for UI Transition on " + name);
                else
                    current.Hide();

                if (target == null)
                    Debug.LogWarning("No current UI Element set for UI Transition on " + name);
                else
                    target.Show();

                yield return Fader.To(0f, duration);
            }
        }
    }
}