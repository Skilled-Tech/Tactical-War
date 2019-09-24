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
        float interval;
        public float Interval { get { return interval; } }

        [SerializeField]
        float duration;
        public float Duration
        {
            get
            {
                return duration;
            }
            set
            {
                if (value < 0f) value = 0f;

                this.duration = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(StatusEffectData))
            {
                var status = (StatusEffectData)obj;

                if (status.type != this.type) return false;
                if (status.duration != this.duration) return false;
                if (status.potency != this.potency) return false;

                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return type.GetHashCode() ^ potency.GetHashCode() ^ duration.GetHashCode();
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
                interval = Min(one.interval, two.interval),
                duration = Max(one.duration, two.duration),
            };
        }

        public static float Max(float one, float two)
        {
            if (one > two)
                return one;

            return two;
        }
        public static float Min(float one, float two)
        {
            if (one < two)
                return one;

            return two;
        }
    }
    
    [Serializable]
    public class StatusEffectInstance
    {
        public StatusEffectType Type { get { return data.Type; } }

        public bool IsDepleted { get { return data.Duration == 0f; } }

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

        float interval;

        public virtual void Process()
        {
            if (IsDepleted)
            {

            }
            else
            {
                interval = Mathf.MoveTowards(interval, 0f, Time.deltaTime);

                if (interval == 0f)
                {
                    interval = data.Interval;
                    Apply();
                }
                
                data.Duration = Mathf.MoveTowards(data.Duration, 0f, Time.deltaTime);
            }
        }

        public void Apply()
        {
            Type.Apply(this);
        }

        public void Stack(StatusEffectData effect, Entity affector)
        {
            this.Affector = affector;

            data += effect;
        }

        public StatusEffectInstance(StatusEffectData data, Entity target, Entity affector)
        {
            this.data = data;
            this.affector = affector;
            this.Target = target;
            this.interval = 0f;
        }
    }
}