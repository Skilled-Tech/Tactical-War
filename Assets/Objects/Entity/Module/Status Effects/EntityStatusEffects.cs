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

        public override void Init()
        {
            base.Init();


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
        public virtual void Add(StatusEffectData effect, Entity affector)
        {
            var target = Find(effect.Type);

            if(target == null)
            {
                target = new StatusEffectInstance(effect, this.Entity, affector);

                List.Add(target);
            }
            else
            {
                target.Stack(effect, affector);
            }

            if (OnAdd != null) OnAdd(target);
        }

        public delegate void RemoveDelegate(StatusEffectType type);
        public event RemoveDelegate OnRemove;
        public virtual void Remove(int index)
        {
            var type = List[index].Type;

            List.RemoveAt(index);

            if (OnRemove != null) OnRemove(type);
        }
        public virtual void Remove(StatusEffectType type)
        {
            for (int i = List.Count - 1; i >= 0; i++)
                if (List[i].Type == type)
                    Remove(i);
        }

        protected virtual void Update()
        {
            for (int i = 0; i < List.Count; i++)
                List[i].Process();
        }
    }
}