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

        public delegate void OperationDelegate(StatusEffectInstance effect);

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

        public event OperationDelegate OnAfflect;
        public virtual void Afflict(StatusEffectData data, Entity affector)
        {
            if (data.Type == null)
            {
                Debug.LogWarning("No Status Effect Type was assigned to the argument data, can't add status effect to " + name);
                return;
            }

            var target = Find(data.Type);

            if (target == null)
            {
                target = Add(data, affector);

                List.Add(target);
            }
            else
            {
                Modify(ref target, data, affector);
            }

            target.Run();

            if (OnAfflect != null) OnAfflect(target);
        }

        public event OperationDelegate OnAdd;
        protected virtual StatusEffectInstance Add(StatusEffectData data, Entity affector)
        {
            var target = new StatusEffectInstance(this.Entity, data, affector);

            RemoveConflicts(data.Type);

            target.OnApply += ApplyCallback;
            target.OnFinish += InstanceFinishedDelegate;

            if (OnAdd != null) OnAdd(target);

            return target;
        }

        public event OperationDelegate OnModify;
        protected virtual void Modify(ref StatusEffectInstance instance, StatusEffectData data, Entity affector)
        {
            instance.Stack(data, affector);

            if (OnModify != null) OnModify(instance);
        }

        void InstanceFinishedDelegate(StatusEffectInstance instance)
        {
            Remove(instance.Type);
        }

        public delegate void RemoveDelegate(StatusEffectInstance type);
        public event OperationDelegate OnRemove;
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

        public event StatusEffectInstance.ApplyDelegate OnApply;
        protected virtual void ApplyCallback(StatusEffectInstance effect)
        {
            if (OnApply != null) OnApply(effect);
        }
    }
}