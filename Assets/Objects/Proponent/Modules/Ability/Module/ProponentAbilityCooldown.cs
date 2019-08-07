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
	public class ProponentAbilityCooldown : Proponent.Module
	{
        [SerializeField]
        protected float duration = 20f;
        public float Duration { get { return duration; } } 

        public float Timer { get; protected set; }

        public float Rate { get { return Timer / duration; } }

        public ProponentAbility Ability { get { return Proponent.Ability; } }

        public event Action OnStateChange;

        public override void Init()
        {
            base.Init();

            Ability.OnUse += OnAbilityUse;
        }

        void OnAbilityUse()
        {
            Begin();
        }

        public event Action OnBegin;
        void Begin()
        {
            Timer = duration;

            StartCoroutine(Procedure());

            if (OnBegin != null) OnBegin();

            if (OnStateChange != null) OnStateChange();
        }
        
        IEnumerator Procedure()
        {
            while(Timer != 0f)
            {
                Timer = Mathf.MoveTowards(Timer, 0f, Time.deltaTime);

                if (OnStateChange != null) OnStateChange();

                yield return new WaitForEndOfFrame();
            }

            End();
        }

        public event Action OnEnd;
        void End()
        {
            if (OnEnd != null) OnEnd();

            if (OnStateChange != null) OnStateChange();
        }
    }
}