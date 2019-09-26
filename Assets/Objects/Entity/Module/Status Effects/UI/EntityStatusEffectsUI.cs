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
	public class EntityStatusEffectsUI : Entity.Module
	{
		[SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        public List<EntityStatusEffectUITemplate> Templates { get; protected set; }

        public EntityStatusEffects StatusEffects => Entity.StatusEffects;

        public override void Configure(Entity reference)
        {
            base.Configure(reference);

            Templates = new List<EntityStatusEffectUITemplate>();
        }

        public override void Init()
        {
            base.Init();

            StatusEffects.OnAdd += AddCallback;
            StatusEffects.OnRemove += RemoveCallback;
        }

        protected virtual EntityStatusEffectUITemplate Create(StatusEffectInstance effect)
        {
            var instance = Instantiate(template, transform);

            var script = instance.GetComponent<EntityStatusEffectUITemplate>();

            script.Init();
            script.Set(effect);

            return script;
        }

        void AddCallback(StatusEffectInstance effect)
        {
            var instance = Create(effect);

            Templates.Add(instance);
        }

        void RemoveCallback(StatusEffectInstance effect)
        {
            for (int i = Templates.Count - 1; i >= 0; i--)
            {
                if (Templates[i].Effect.Type == effect.Type)
                {
                    Destroy(Templates[i].gameObject);

                    Templates.RemoveAt(i);
                }
            }
        }
    }
}