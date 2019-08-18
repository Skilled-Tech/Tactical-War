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
	public abstract class UnitAttack : Unit.Module
	{
        [SerializeField]
        protected float damage = 20f;
        public float Damage { get { return damage; } }

        public virtual float SampleDamage()
        {
            return DamageUpgrade.Sample(damage);
        }

        [SerializeField]
        protected float duration = 1f;
        public float Duration { get { return duration; } }

        public ProponentUpgradeProperty DamageUpgrade;

        public override void Init()
        {
            base.Init();

            Body.AnimationEvent.OnTrigger += OnAnimationTrigger;

            DamageUpgrade = Leader.Upgrades.Contexts[0].Properties[0];
        }

        void OnAnimationTrigger(string ID)
        {
            switch (ID)
            {
                case "Attack Connected":
                    AttackConnected();
                    break;

                default:
                    break;
            }
        }

        public virtual void DoDamage(Entity target)
        {
            Unit.DoDamage(target, SampleDamage());
        }

        public Entity Target { get; protected set; }
        public virtual Coroutine Do(Entity target)
        {
            this.Target = target;

            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(Procedure());

            return coroutine;
        }

        Coroutine coroutine;
        public bool IsProcessing { get { return coroutine != null; } }
        IEnumerator Procedure()
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.5f));

            Body.Animator.SetTrigger("Attack");

            yield return new WaitForSeconds(duration);

            Target = null;

            coroutine = null;
        }

        public virtual void AttackConnected()
        {
            
        }
    }
}