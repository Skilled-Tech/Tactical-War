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
        public Entity Target
        {
            get
            {
                if (Enemy.Base.Units.Count == 0)
                    return Enemy.Base;

                return Enemy.Base.Units.First;
            }
        }

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
                    display_value = Mathf.RoundToInt(base.Value);

                    return Mathf.RoundToInt(base.Value);
                }
            }

            public int display_value;
        }

        public float Distance
        {
            get
            {
                return Template.Attack.Distance * range.Multiplier;
            }
        }

        public float DistanceTo(Entity target)
        {
            return Unit.Bounds.extents.x + target.Bounds.extents.x;
        }
        #endregion

        public Damage.Method Method { get { return Template.Attack.Method; } }

        public float Duration { get { return Template.Attack.Duration; } }

        public Proponent Enemy { get { return Leader.Enemy; } }

        public class Module : Module<UnitAttack>
        {
            public UnitAttack Attack { get { return Reference; } }

            public Unit Unit { get { return Attack.Unit; } }

            public Proponent Leader { get { return Unit.Leader; } }
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

        public virtual bool CanPerform
        {
            get
            {
                if (IsProcessing) return false;

                if (Index >= Range.Value) return false;

                if(Index > 0 && Leader.Units.First.Controller.isMoving) return false;

                if (Target == null) return false;

                if (Controller.isMoving) return false;

                return true;
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

        public event Action OnInitiate;
        protected virtual void Initiate()
        {
            audio.SFX.PlayOneShot(Template.Attack.SFX.Initiate);

            if (OnInitiate != null) OnInitiate();
        }

        IEnumerator Procedure()
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.5f));

            Initiate();

            yield return new WaitForSeconds(Duration);

            coroutine = null;
        }

        public event Action OnConnected;
        protected virtual void Connected()
        {
            audio.SFX.PlayOneShot(Template.Attack.SFX.Connect);

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