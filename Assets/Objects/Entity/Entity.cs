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
    public class Entity : MonoBehaviour
    {
        public Health Health { get; protected set; }
        public bool Injured { get { return Health.Value != Health.Max; } }
        public bool IsAlive { get { return Health.Value > 0f; } }
        public bool IsDead { get { return Health.Value == 0f; } }

        public EntityDefense Defense { get; protected set; }

        public EntityStatusEffects StatusEffects { get; protected set; }

        public EntityTimeScale TimeScale { get; protected set; }

        public abstract class Module : Module<Entity>
        {
            public Entity Entity { get { return Reference; } }
        }

        public Bounds Bounds { get; protected set; }
        public virtual void CalculateBounds() => Bounds = Tools.Bounds.FromCollider2D(gameObject);
        public Vector3 Center
        {
            get
            {
                return transform.TransformPoint(Bounds.center);
            }
        }
        public Vector3 Size => Bounds.size;

        public virtual float PersonalSpace => Bounds.extents.x;
        
        protected virtual void Awake()
        {
            Health = Dependancy.Get<Health>(gameObject);
            Defense = Dependancy.Get<EntityDefense>(gameObject);
            StatusEffects = Dependancy.Get<EntityStatusEffects>(gameObject);
            TimeScale = Dependancy.Get<EntityTimeScale>(gameObject);

            Modules.Configure(this);
        }

        protected virtual void Start()
        {
            Modules.Init(this);

            CalculateBounds();
        }

        public delegate void DoDamageDelegate(Damage.Result result);
        public event DoDamageDelegate OnDoDamage;
        public virtual Damage.Result DoDamage(float value, Damage.Method method, Entity target)
        {
            var result = target.TakeDamage(value, method, this);

            if (OnDoDamage != null) OnDoDamage(result);

            return result;
        }

        public delegate void TakeDamageDelegate(Damage.Result result);
        public event TakeDamageDelegate OnTookDamage;
        protected virtual Damage.Result TakeDamage(float value, Damage.Method method, Entity cause)
        {
            if (Defense != null)
                value = Defense.Sample(value);

            var initialHealth = Health.Value;

            if(Health.Value > 0f)
            {
                Health.Value -= value;
            }
            else
            {

            }

            var result = new Damage.Result(value, this, method, cause);

            if (OnTookDamage != null) OnTookDamage(result);

            if(initialHealth > 0)
            {
                if(IsDead)
                    Death(result);
            }

            return result;
        }

        public virtual void Suicide()
        {
            DoDamage(Health.Value, Damage.Method.Contact, this);
        }
        public virtual void Sudoku() => Suicide(); //Just like suicide but less tragic

        public delegate void DeathDelegate(Damage.Result result);
        public event DeathDelegate OnDeath;
        protected virtual void Death(Damage.Result result)
        {
            if (OnDeath != null) OnDeath(result);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(Bounds.center, Bounds.size);
        }

        public virtual float DistanceTo(Entity entity)
        {
            return DistanceTo(entity.Center);
        }
        public virtual float DistanceTo(Vector3 position)
        {
            return Vector3.Distance(transform.position, position);
        }
    }

    public class Damage
    {
        public struct Request
        {
            public float Value { get; private set; }

            public Method Method { get; private set; }

            public Entity Source { get; private set; }
            public Entity Target { get; private set; }

            public Request(float value, Entity target, Method method, Entity source)
            {
                this.Value = value;

                this.Target = target;

                this.Method = method;

                this.Source = source;
            }
        }

        public struct Result
        {
            public float Value { get; private set; }

            public Method Method { get; private set; }

            public Entity Source { get; private set; }
            public Entity Target { get; private set; }

            public Result(float value, Entity target, Method method, Entity source)
            {
                this.Value = value;

                this.Target = target;

                this.Method = method;

                this.Source = source;
            }

            public Result(Request request) : this(request.Value, request.Target, request.Method, request.Source)
            {

            }
        }

        public enum Method
        {
            Contact,
            Effect,
            Ranged
        }
    }
}