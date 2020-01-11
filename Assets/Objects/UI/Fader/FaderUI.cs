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
    [RequireComponent(typeof(Image))]
	public class FaderUI : UIElement
	{
		public Image Image { get; protected set; }

        public float Value
        {
            get
            {
                return Image.color.a;
            }
            set
            {
                var color = Image.color;
                color.a = value;
                Image.color = color;

                Image.raycastTarget = value > 0f;
            }
        }

        public const float Duration = 0.4f;

        private void Awake()
        {
            Image = GetComponent<Image>();
        }

        public virtual Coroutine To(float target) => To(target, Duration);
        public virtual Coroutine To(float target, float duration)
        {
            IEnumerator Procedure()
            {
                var timer = duration;

                var initialValue = Value;

                while (timer > 0f)
                {
                    timer = Mathf.MoveTowards(timer, 0f, Time.unscaledDeltaTime);

                    Value = Mathf.Lerp(target, initialValue, timer / duration);

                    yield return new WaitForEndOfFrame();
                }
            }

            return StartCoroutine(Procedure());
        }

        public virtual Coroutine Operate(Action action)
        {
            return StartCoroutine(Procedure());

            IEnumerator Procedure()
            {
                yield return To(1f, 0.2f);

                action();

                yield return new WaitForSecondsRealtime(0.2f);

                yield return To(0f, 0.2f);
            }
        }
    }
}