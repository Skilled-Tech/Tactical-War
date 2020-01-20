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
	public class EntityParalysis : EntityStatusEffectImplementation<ParalysisStatusEffect>, EntityTimeScale.IModifer
	{
		public float Percentage { get; protected set; }

        public override void Configure(Entity data)
        {
            base.Configure(data);

            Percentage = 0f;
        }

        public override void Init()
        {
            base.Init();

            StatusEffects.OnApply += OnApply;
        }

        void OnApply(StatusEffectInstance instance)
        {
            if (instance.Type == type)
            {
                if(Entity.IsAlive)
                {
                    if (coroutine != null)
                        StopCoroutine(coroutine);

                    coroutine = StartCoroutine(Procedure());
                }
            }
        }

        Coroutine coroutine;
        IEnumerator Procedure()
        {
            Percentage = -100f;

            yield return new WaitForSeconds(type.Delay);

            Percentage = 0f;

            coroutine = null;
        }
    }
}