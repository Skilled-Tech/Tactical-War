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
    public abstract class EntityStatusEffectImplementation<TType> : Entity.Module
        where TType : StatusEffectType
    {
        [SerializeField]
        protected TType type;
        public TType Type { get { return type; } }

        public StatusEffectInstance Effect { get; protected set; }

        public EntityStatusEffects StatusEffects => Entity.StatusEffects;

        public override void Init()
        {
            base.Init();

            StatusEffects.OnAdd += OnAdd;
            StatusEffects.OnRemove += OnRemove;
        }

        protected virtual void OnAdd(StatusEffectInstance effect)
        {
            if (effect.Type == this.type)
                this.Effect = effect;
        }

        protected virtual void OnRemove(StatusEffectInstance effect)
        {
            if (effect.Type == this.type)
                this.Effect = null;
        }
    }
}