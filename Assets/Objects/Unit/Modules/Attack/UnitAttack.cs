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
        public Damage.Method Method { get { return Template.Attack.Method; } }

        #region Damage
        public float BaseDamage { get { return Unit.Template.Attack.Damage; } }

        public float DamageMultiplier
        {
            get
            {
                return 1f + Upgrades.Damage / 100f;
            }
        }

        public float Damage { get { return BaseDamage * DamageMultiplier; } }
        #endregion

        #region Range
        public uint BaseRange { get { return Unit.Template.Attack.Range; } }

        public uint RangeIncrease
        {
            get
            {
                return (uint)(Upgrades.Range / 50f);
            }
        }

        public uint Range { get { return BaseRange + RangeIncrease; } }
        #endregion

        #region Distance
        public float BaseDistance { get { return Unit.Template.Attack.Distance; } }

        public float DistanceMultiplier
        {
            get
            {
                return 1f + Upgrades.Range / 100f;
            }
        }

        public float Distance { get { return BaseDistance * DistanceMultiplier; } }
        #endregion

        public float Duration { get { return Template.Attack.Duration; } }

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
            Unit.DoDamage(Damage, Method, target);
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

            yield return new WaitForSeconds(Duration);

            Target = null;

            coroutine = null;
        }

        public virtual void AttackConnected()
        {
            
        }
    }
}