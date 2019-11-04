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
	public class WindBlastAbility : Ability
	{
        [SerializeField]
        protected float duration = 1f;
        public float Duration { get { return duration; } }

        public override void Init()
        {
            base.Init();

            StartCoroutine(Procedure());
        }

        IEnumerator Procedure()
        {
            var timer = duration;

            while(timer > 0f)
            {
                timer = Mathf.MoveTowards(timer, 0f, Time.deltaTime);

                for (int i = 0; i < User.Enemy.Units.Count; i++)
                {
                    var target = User.Enemy.Units[i];

                    target.transform.position = Vector3.Lerp(User.Enemy.Base.Units.Creator.transform.position, target.transform.position, timer / duration);
                }

                yield return new WaitForEndOfFrame();
            }

            End();

            yield break;
        }
    }
}