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
	public class EntityStatusEffectParticles : Entity.Module
	{
		[SerializeField]
        protected ParticleSystem system;
        public ParticleSystem System { get { return system; } }

        public virtual void Stop()
        {
            system.Stop(true);
        }

        public virtual void Play()
        {
            system.Play(true);
        }

        [SerializeField]
        protected StatusEffectType type;
        public StatusEffectType Type { get { return type; } }

        protected virtual void Reset()
        {
            system = Dependancy.Get<ParticleSystem>(gameObject);
        }

        public override void Init()
        {
            base.Init();

            Stop();

            Entity.StatusEffects.OnAdd += AddCallback;
            Entity.StatusEffects.OnRemove += RemoveCallback;
        }

        void AddCallback(StatusEffectInstance effect)
        {
            if (effect.Type == type)
                Play();
        }

        void RemoveCallback(StatusEffectInstance effect)
        {
            if (effect.Type == type)
                Stop();
        }
    }
}