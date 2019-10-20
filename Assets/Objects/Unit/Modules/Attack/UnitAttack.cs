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
        #region Properties
        [SerializeField]
        protected PowerProperty power;
        public PowerProperty Power { get { return power; } }
        [Serializable]
        public class PowerProperty : UnitUpgrades.Property
        {
            public override ItemUpgradeType Type => Core.Items.Upgrades.Types.Common.Power;

            public override float Base => Template.Attack.Power;
        }

        [SerializeField]
        protected RangeProperty range;
        public RangeProperty Range { get { return range; } }
        [Serializable]
        public class RangeProperty : UnitUpgrades.Property
        {
            public override ItemUpgradeType Type => Core.Items.Upgrades.Types.Common.Range;

            public override float Base => Template.Attack.Range;

            new public int Value
            {
                get
                {
                    return Mathf.RoundToInt(base.Value);
                }
            }
        }

        public float Distance
        {
            get
            {
                return Template.Attack.Distance * range.Multiplier;
            }
        }
        #endregion

        public Damage.Method Method { get { return Template.Attack.Method; } }

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

                Attack.OnInitiate += OnInitiated;
                Attack.OnConnected += OnConnected;
            }

            protected virtual void DoDamage(Entity entity)
            {
                Attack.DoDamage(entity);
            }

            protected virtual void OnInitiated()
            {

            }

            protected virtual void OnConnected()
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

            power.Init(Unit);
            range.Init(Unit);

            Body.AnimationEvents.OnCustomEvent += OnAnimationTrigger;

            Modules.Init(this);
        }

        protected virtual void OnAnimationTrigger(string ID)
        {
            if (ID == "Hit")
                Connected();
        }

        public virtual Coroutine Perform()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(Procedure());

            return coroutine;
        }

        Coroutine coroutine;
        public bool IsProcessing { get { return coroutine != null; } }

        public event Action OnInitiate;

        IEnumerator Procedure()
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.5f));

            if (OnInitiate != null) OnInitiate();

            yield return new WaitForSeconds(Duration);

            coroutine = null;
        }

        public event Action OnConnected;
        public virtual void Connected()
        {
            if (OnConnected != null) OnConnected();
        }

        public event Entity.DoDamageDelegate OnDoDamage;
        public virtual Damage.Result DoDamage(Entity target)
        {
            var result = Unit.DoDamage(Power.Value, Method, target);

            ApplyStatusEffects(Template.Attack.StatusEffects, target);

            if (OnDoDamage != null) OnDoDamage(result);

            return result;
        }

        protected virtual void ApplyStatusEffects(IList<UnitTemplate.StatusEffectProperty> list, Entity target)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Unit.Upgrades.GetElements(list[i].Upgrade, out var template, out var data);

                var rate = data == null ? 0f : (data.Value / 1f / template.Ranks.Length);

                var probability = list[i].Probability.Sample(rate);

                if (StatusEffect.CheckProbability(probability))
                {
                    StatusEffect.Afflict(list[i].Data, target, this.Unit);
                }
                else
                {

                }
            }
        }
    }
}