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
	public class ProponentAbilityCooldown : ProponentAbility.Module
	{
        public float Timer { get; protected set; }

        public float Duration => Ability.Template.Usage.Cooldown;

        public float Rate => Timer / Duration;

        public override void Configure(ProponentAbility reference)
        {
            base.Configure(reference);

            Timer = 0f;
        }

        public void Begin()
        {
            this.Timer = Duration;

            StartCoroutine(Procedure());
        }

        IEnumerator Procedure()
        {
            while (Timer > 0f)
            {
                yield return new WaitForEndOfFrame();

                Tick();
            }
        }

        public event Action OnTick;
        void Tick()
        {
            Timer = Mathf.MoveTowards(Timer, 0f, Time.deltaTime);

            if (OnTick != null) OnTick();
        }

        //Static Utility
        public static ProponentAbilityCooldown Create(ProponentAbility ability)
        {
            var instance = new GameObject("Cooldown");

            instance.transform.SetParent(ability.transform);

            var script = instance.AddComponent<ProponentAbilityCooldown>();

            return script;
        }
    }
}