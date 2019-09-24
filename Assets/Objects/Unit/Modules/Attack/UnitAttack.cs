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
	public class UnitAttack : Unit.Module
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

        public class Module : Module<UnitAttack>
        {
            public UnitAttack Attack { get { return Reference; } }

            public Unit Unit { get { return Attack.Unit; } }

            public Proponent Leader { get { return Unit.Leader; } }

            public Proponent Enemy { get { return Leader.Enemy; } }

            public override void Init()
            {
                base.Init();

                Attack.OnAttackConnected += AttackConnected;
            }

            protected virtual void DoDamage(Entity entity)
            {
                Attack.DoDamage(entity);
            }

            protected virtual void AttackConnected()
            {
                
            }
        }

        public override void Configure(Unit data)
        {
            base.Configure(data);

            Modules.Configure(this);
        }

        public override void Init()
        {
            base.Init();

            Body.AnimationEvents.OnCustomEvent += OnAnimationTrigger;

            Modules.Init(this);
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

        public event Entity.DoDamageDelegate OnDoDamage;
        public virtual Damage.Result DoDamage(Entity target)
        {
            var result = Unit.DoDamage(Damage, Method, target);

            if (OnDoDamage != null) OnDoDamage(result);

            return result;
        }

        public virtual Coroutine Initiate()
        {
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

            coroutine = null;
        }

        public event Action OnAttackConnected;
        public virtual void AttackConnected()
        {
            if (OnAttackConnected != null) OnAttackConnected();
        }
    }
}