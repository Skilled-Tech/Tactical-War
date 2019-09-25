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
	public class EntityStatusEffects : Entity.Module
	{
        public List<StatusEffectInstance> List { get; protected set; }

        public override void Configure(Entity data)
        {
            base.Configure(data);

            List = new List<StatusEffectInstance>();
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

                target.OnApply += ApplyCallback;

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

        public event StatusEffectInstance.ApplyDelegate OnApply;
        protected virtual void ApplyCallback(StatusEffectInstance effect)
        {
            if (OnApply != null) OnApply(effect);
        }
    }
}