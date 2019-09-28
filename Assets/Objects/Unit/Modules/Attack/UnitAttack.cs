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

        #region Properties
        [SerializeField]
        protected DamageProperty damage;
        public DamageProperty Damage { get { return damage; } }
        [Serializable]
        public class DamageProperty : Property<float>
        {
            public override float Base => Template.Attack.Damage;

            public override float Value => Base * Multiplier;
        }

        [SerializeField]
        protected RangeProperty range;
        public RangeProperty Range { get { return range; } }
        [Serializable]
        public class RangeProperty : Property<int>
        {
            public override float Base => Template.Attack.Range;

            public override int Value => Mathf.RoundToInt(Base * Multiplier);
        }
        public int BaseRange { get { return Unit.Template.Attack.Range; } }

        public float Distance
        {
            get
            {
                return Template.Attack.Distance * range.Multiplier;
            }
        }

        [Serializable]
        public abstract class Property<TValue>
        {
            [SerializeField]
            protected ItemUpgradeType upgrade;
            public ItemUpgradeType Upgrade { get { return upgrade; } }

            public ItemUpgradesTemplate.ElementData.RankData GetCurrentRank()
            {
                return Upgrades.FindCurrentRank(upgrade);
            }

            public abstract float Base { get; }

            public virtual float Percentage
            {
                get
                {
                    var rank = GetCurrentRank();

                    if (rank == null)
                        return 0f;

                    return rank.Percentage;
                }
            }

            public virtual float Multiplier => 1f + (Percentage / 100f); 

            public abstract TValue Value { get; }

            protected UnitAttack attack;

            public Unit Unit { get { return attack.Unit; } }
            public Proponent Leader { get { return Leader; } }
            public UnitUpgrades Upgrades { get { return Unit.Upgrades; } }

            public UnitTemplate Template { get { return Unit.Template; } }

            public virtual void Init(UnitAttack attack)
            {
                this.attack = attack;
            }
        }
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

            damage.Init(this);
            range.Init(this);

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

        public virtual Coroutine Perform()
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

        public event Entity.DoDamageDelegate OnDoDamage;
        public virtual Damage.Result DoDamage(Entity target)
        {
            var result = Unit.DoDamage(Damage.Value, Method, target);

            ApplyStatusEffects(Template.Attack.StatusEffects, target);

            if (OnDoDamage != null) OnDoDamage(result);

            return result;
        }

        protected virtual void ApplyStatusEffects(IList<UnitTemplate.StatusEffectProperty> list, Entity target)
        {
            for (int i = 0; i < list.Count; i++)
            {
                ItemUpgradesTemplate.ElementData template;
                ItemUpgradesData.ElementData data;

                Unit.Upgrades.GetElements(list[i].Upgrade, out template, out data);

                var probability = list[i].Probability.Sample(data.Value / 1f / template.Ranks.Length);

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