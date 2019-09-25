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
    public static class StatusEffect
    {
        public const string MenuPath = "Status Effect/";

        public static bool Afflect(StatusEffectData data, Entity target, Entity affector)
        {
            if (target == null) //Target Died ?
                return false;

            if (target.StatusEffects == null)
                return false;

            target.StatusEffects.Add(data, affector);

            return true;
        }
    }
    
    [Serializable]
    public struct StatusEffectData
    {
        [SerializeField]
        StatusEffectType type;
        public StatusEffectType Type { get { return type; } }

        [SerializeField]
        float potency;
        public float Potency { get { return potency; } }

        [SerializeField]
        int iterations;
        public int Iterations { get { return iterations; } }

        [SerializeField]
        float interval;
        public float Interval
        {
            get
            {
                return interval;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(StatusEffectData))
            {
                var status = (StatusEffectData)obj;

                if (status.type != this.type) return false;
                if (status.iterations != this.iterations) return false;
                if (status.interval != this.interval) return false;
                if (status.potency != this.potency) return false;

                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return type.GetHashCode() ^ potency.GetHashCode() ^ iterations.GetHashCode() ^ interval.GetHashCode();
        }

        public static bool operator ==(StatusEffectData one, StatusEffectData two)
        {
            return one.Equals(two);
        }
        public static bool operator !=(StatusEffectData one, StatusEffectData two)
        {
            return !one.Equals(two);
        }

        public static StatusEffectData operator +(StatusEffectData one, StatusEffectData two)
        {
            return new StatusEffectData()
            {
                type = two.type,
                potency = Max(one.potency, two.potency),
                iterations = Max(one.iterations, two.iterations),
                interval = Max(one.interval, two.interval),
            };
        }

        public static float Max(float one, float two)
        {
            if (one > two)
                return one;

            return two;
        }
        public static int Max(int one, int two)
        {
            if (one > two)
                return one;

            return two;
        }
    }
    
    [Serializable]
    public class StatusEffectInstance
    {
        public StatusEffectType Type { get { return data.Type; } }

        public bool IsDepleted { get { return Iterations >= data.Iterations; } }

        protected StatusEffectData data;
        public StatusEffectData Data { get { return data; } }

        public Entity Target { get; protected set; }

        protected Entity affector;
        public Entity Affector
        {
            get
            {
                return affector;
            }
            set
            {
                if (affector == value)
                {

                }
                else
                {
                    affector = value;

                    if (OnAffectorChange != null) OnAffectorChange(Affector);
                }
            }
        }
        public delegate void AffectorChangeDelegate(Entity affector);
        public event AffectorChangeDelegate OnAffectorChange;

        public float Iterations { get; protected set; }

        public virtual void Run()
        {
            if (coroutine != null)
                Target.StopCoroutine(coroutine);

            coroutine = Target.StartCoroutine(Procedure());
        }

        public delegate void FinishDelegate(StatusEffectInstance instance);
        public event FinishDelegate OnFinish;
        Coroutine coroutine;
        IEnumerator Procedure()
        {
            Iterations = 0;

            Apply(); //Always apply first

            yield return new WaitForSeconds(data.Interval);

            while (IsDepleted == false)
            {
                Apply();

                yield return new WaitForSeconds(data.Interval);
            }

            coroutine = null;

            if (OnFinish != null) OnFinish(this);
        }

        public delegate void ApplyDelegate(StatusEffectInstance instance);
        public event ApplyDelegate OnApply;
        protected virtual void Apply()
        {
            Iterations++;

            Type.Apply(this);

            if (OnApply != null) OnApply(this);
        }

        public virtual void Stack(StatusEffectData effect, Entity affector)
        {
            this.Affector = affector;

            data += effect;
        }

        public StatusEffectInstance(Entity target)
        {
            this.Target = target;
        }
    }
}