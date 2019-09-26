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
	public class EntityStatusEffectParticles : EntityStatusEffects.ActivationModule
	{
        [SerializeField]
        protected StatusEffectType type;
        public StatusEffectType Type { get { return type; } }

        [SerializeField]
        protected ParticleSystem system;
        public ParticleSystem System { get { return system; } }

        [SerializeField]
        protected ConditionsData conditions = ConditionsData.Defaults;
        public ConditionsData Conditions { get { return conditions; } }
        [Serializable]
        public struct ConditionsData
        {
            [SerializeField]
            EntityStatusEffects.ActivationCondition[] play;
            public EntityStatusEffects.ActivationCondition[] Play { get { return play; } }
            public bool IsPlayCondition(EntityStatusEffects.ActivationCondition condition)
            {
                return play.Contains(condition);
            }

            [SerializeField]
            EntityStatusEffects.ActivationCondition[] stop;
            public EntityStatusEffects.ActivationCondition[] Stop { get { return stop; } }
            public bool IsStopCondition(EntityStatusEffects.ActivationCondition condition)
            {
                return stop.Contains(condition);
            }

            public bool Contains(EntityStatusEffects.ActivationCondition condition)
            {
                if (play.Contains(condition)) return true;

                if (stop.Contains(condition)) return true;

                return false;
            }

            public static ConditionsData Defaults
            {
                get
                {
                    return new ConditionsData
                    {
                        play = new EntityStatusEffects.ActivationCondition[] { EntityStatusEffects.ActivationCondition.OnAdd },
                        stop = new EntityStatusEffects.ActivationCondition[] { EntityStatusEffects.ActivationCondition.OnRemove }
                    };
                }
            }
        }
        public override bool IsValidCondition(EntityStatusEffects.ActivationCondition condition)
        {
            return conditions.Contains(condition);
        }

        protected virtual void Reset()
        {
            system = Dependancy.Get<ParticleSystem>(gameObject);
        }

        public virtual void Stop()
        {
            system.Stop(true);
        }
        public virtual void Play()
        {
            system.Play(true);
        }

        public override void Init()
        {
            base.Init();

            Stop();
        }

        protected override void Process(EntityStatusEffects.ActivationCondition condition)
        {
            if (conditions.IsPlayCondition(condition))
                Play();

            if (conditions.IsStopCondition(condition))
                Stop();
        }
    }
}