﻿using System;
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
	public class EntityStatusEffects : Entity.Module
	{
        [SerializeField]
        protected DefaultsData defaults;
        public DefaultsData Defaults { get { return defaults; } }
        [Serializable]
        public class DefaultsData
        {
            [SerializeField]
            protected InstanceData poison;
            public InstanceData Poison { get { return poison; } }

            [SerializeField]
            protected InstanceData fire;
            public InstanceData Fire { get { return fire; } }

            [SerializeField]
            protected InstanceData chill;
            public InstanceData Chill { get { return chill; } }

            [Serializable]
            public class InstanceData
            {
                [SerializeField]
                protected StatusEffectType type;
                public StatusEffectType Type { get { return type; } }

                public StatusEffectInstance Instance { get; protected set; }

                public float Potency
                {
                    get
                    {
                        if (Instance == null)
                            return 0f;

                        return Instance.Data.Potency;
                    }
                }

                public virtual void Update(StatusEffectInstance instance)
                {
                    this.Instance = instance;
                }
            }

            public virtual InstanceData Find(StatusEffectType type)
            {
                if (type == poison.Type) return poison;
                if (type == fire.Type) return fire;
                if (type == chill.Type) return chill;

                return null;
            }

            public virtual void Init(EntityStatusEffects statusEffects)
            {
                statusEffects.OnAdd += OnAdd;
                statusEffects.OnRemove += OnRemove;
            }

            void OnAdd(StatusEffectInstance effect)
            {
                var instance = Find(effect.Type);

                if (instance == null)
                {
                    
                }
                else
                {
                    instance.Update(effect);
                }
            }

            void OnRemove(StatusEffectInstance effect)
            {
                var instance = Find(effect.Type);

                if(instance == null)
                {
                    
                }
                else
                {
                    instance.Update(null);
                }
            }
        }

        public List<StatusEffectInstance> List { get; protected set; }

        public override void Configure(Entity data)
        {
            base.Configure(data);

            List = new List<StatusEffectInstance>();
        }

        public override void Init()
        {
            base.Init();

            defaults.Init(this);
        }

        public virtual StatusEffectInstance Find(StatusEffectType type)
        {
            for (int i = 0; i < List.Count; i++)
                if (List[i].Type == type)
                    return List[i];

            return null;
        }

        public virtual bool Contains(StatusEffectType type)
        {
            for (int i = 0; i < List.Count; i++)
                if (List[i].Type == type)
                    return true;

            return false;
        }

        public delegate void AddDelegate(StatusEffectInstance effect);
        public event AddDelegate OnAdd;
        public virtual void Add(StatusEffectData data, Entity affector)
        {
            if(data.Type == null)
            {
                Debug.LogWarning("No Status Effect Type was assigned to the argument data, can't add status effect to " + name);
                return;
            }

            var target = Find(data.Type);

            if(target == null)
            {
                target = new StatusEffectInstance(data, this.Entity, affector);

                List.Add(target);
            }
            else
            {
                target.Stack(data, affector);
            }

            RemoveConflicts(data.Type);

            if (OnAdd != null) OnAdd(target);
        }

        public delegate void RemoveDelegate(StatusEffectInstance type);
        public event RemoveDelegate OnRemove;
        public virtual void Remove(int index)
        {
            var instance = List[index];

            List.RemoveAt(index);

            if (OnRemove != null) OnRemove(instance);
        }
        public virtual void Remove(StatusEffectType type)
        {
            for (int i = List.Count - 1; i >= 0; i--)
                if (List[i].Type == type)
                    Remove(i);
        }
        protected virtual void RemoveConflicts(StatusEffectType type)
        {
            for (int i = List.Count - 1; i >= 0; i--)
                if (type.ConflictsWith(List[i].Type))
                    Remove(i);
        }

        protected virtual void Update()
        {
            for (int i = List.Count - 1; i >= 0; i--)
            {
                List[i].Process();

                if (List[i].IsDepleted)
                    Remove(i);
            }
        }
    }
}