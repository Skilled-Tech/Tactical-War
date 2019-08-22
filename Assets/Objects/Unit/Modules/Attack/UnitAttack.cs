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
        #region Damage
        public float BaseDamage { get { return Unit.Data.Attack.Damage; } }

        public float DamageMultiplier { get; set; } = 1f;

        public float Damage { get { return BaseDamage * DamageMultiplier; } }
        #endregion

        #region Range
        public uint BaseRange { get { return Unit.Data.Attack.Range; } }

        public uint RangeIncrease { get; set; } = 0;

        public uint Range { get { return BaseRange + RangeIncrease; } }
        #endregion

        #region Distance
        public float BaseDistance { get { return Unit.Data.Attack.Distance; } }

        public float DistanceMultiplier { get; set; } = 1f;

        public float Distance { get { return BaseDistance * DistanceMultiplier; } }
        #endregion

        [SerializeField]
        protected float duration = 1f;
        public float Duration { get { return duration; } }

        public override void Init()
        {
            base.Init();

            Body.AnimationEvents.OnCustomEvent += OnAnimationTrigger;
        }

        void OnAnimationTrigger(string ID)
        {
            switch (ID)
            {
                case "Hit":
                    AttackConnected();
                    break;

                default:
                    break;
            }
        }

        public virtual void DoDamage(Entity target)
        {
            Unit.DoDamage(target, Damage);
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

            Body.CharacterAnimation.Attack();

            yield return new WaitForSeconds(duration);

            Target = null;

            coroutine = null;
        }

        public virtual void AttackConnected()
        {
            
        }
    }
}