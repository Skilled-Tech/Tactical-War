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
        public bool IsAlive { get { return Health.Value > 0f; } }
        public bool IsDead { get { return Health.Value == 0f; } }

        public abstract class Module : Module<Entity>
        {
            public Entity Entity { get { return Data; } }
        }

        protected virtual void Awake()
        {
            Health = Dependancy.Get<Health>(gameObject);

            Modules.Configure(this);
        }

        protected virtual void Start()
        {
            Modules.Init(this);
        }

        public void DoDamage(Entity target, float damage)
        {
            target.TakeDamage(this, damage);
        }

        public delegate void TakeDamageDelegate(Entity damager, float value);
        public event TakeDamageDelegate OnTookDamage;
        public void TakeDamage(Entity damager, float value)
        {
            if (Health.Value == 0) return;

            Health.Value -= value;

            if (OnTookDamage != null) OnTookDamage(damager, value);

            if(Health.Value == 0)
                Death(damager);
        }

        public delegate void DeathDelegate(Entity damager);
        public event DeathDelegate OnDeath;
        protected virtual void Death(Entity damager)
        {
            if (OnDeath != null) OnDeath(damager);
        }
	}
}