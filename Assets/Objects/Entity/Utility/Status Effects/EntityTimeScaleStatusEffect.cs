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
	public class EntityTimeScaleStatusEffect : Entity.Module, EntityTimeScale.IModifer
	{
        [SerializeField]
        protected StatusEffectType type;
        public StatusEffectType Type { get { return type; } }

        public StatusEffectInstance Effect { get; protected set; }

        public EntityStatusEffects StatusEffects => Entity.StatusEffects;

        float EntityTimeScale.IModifer.Value
        {
            get
            {
                if (StatusEffects == null) return 0f;

                return Effect.Data.Potency;
            }
        }

        public override void Init()
        {
            base.Init();

            StatusEffects.OnAdd += OnAdd;
            StatusEffects.OnRemove += OnRemove;
        }

        void OnAdd(StatusEffectInstance effect)
        {
            
        }

        void OnRemove(StatusEffectInstance type)
        {
            
        }
    }
}