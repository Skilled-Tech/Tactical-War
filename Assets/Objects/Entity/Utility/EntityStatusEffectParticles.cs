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

        public bool Emission
        {
            set
            {
                var module = system.emission;

                module.enabled = value;
            }
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

            UpdateState();

            Entity.StatusEffects.OnAdd += AddCallback;
            Entity.StatusEffects.OnRemove += RemoveCallback;
        }

        void UpdateState()
        {
            Emission = Entity.StatusEffects.Contains(type);
        }

        void AddCallback(StatusEffectInstance effect)
        {
            UpdateState();
        }

        void RemoveCallback(StatusEffectType type)
        {
            UpdateState();
        }
    }
}