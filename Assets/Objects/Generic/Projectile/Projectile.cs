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

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
namespace Game
{
    [RequireComponent(typeof(Rigidbody2D))]
	public class Projectile : MonoBehaviour
	{
        public Rigidbody2D rigidbody { get; protected set; }
        public void SetVelocity(float velocity)
        {
            rigidbody.velocity = transform.right * velocity;
        }

        public class Module : Module<Projectile>
        {
            public Projectile Projectile { get { return Reference; } }

            public Entity Owner { get { return Projectile.Owner; } }
        }
        public abstract class ActivationModule : Module
        {
            [SerializeField]
            protected ActivationCondition[] conditions = new ActivationCondition[] { ActivationCondition.OnHit };
            public ActivationCondition[] Conditions { get { return conditions; } }

            public override void Init()
            {
                base.Init();

                Projectile.OnCollision += CollisionCallback;
                Projectile.OnHit += HitCallback;
                Projectile.DestroyEvent += DestroyCallback;
            }

            protected virtual void HitCallback(Collider2D collider)
            {
                if (conditions.Contains(Projectile.ActivationCondition.OnHit))
                    Process();
            }

            protected virtual void CollisionCallback(Collision2D collision)
            {
                if (conditions.Contains(Projectile.ActivationCondition.OnCollision))
                    Process();
            }

            protected virtual void DestroyCallback()
            {
                if (conditions.Contains(Projectile.ActivationCondition.OnDestroy))
                    Process();
            }

            protected abstract void Process();
        }

        public enum ActivationCondition
        {
            OnCollision, OnHit, OnDestroy
        }

        public Entity Owner { get; protected set; }

        public virtual void Configure(Entity owner)
        {
            this.Owner = owner;

            rigidbody = GetComponent<Rigidbody2D>();

            Modules.Configure(this);
        }

        public virtual void SetLayer(string layer)
        {
            SetLayer(LayerMask.NameToLayer(layer));
        }
        public virtual void SetLayer(int layer)
        {
            Tools.Layer.Set(gameObject, layer);
        }

        public virtual void AmmendLayer(int layer)
        {
            SetLayer(LayerMask.LayerToName(layer) + " " + nameof(Projectile));
        }

        protected virtual void Start()
        {
            Modules.Init(this);
        }

        public delegate void CollisionDelegate(Collision2D collision);
        public event CollisionDelegate OnCollision;
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (OnCollision != null) OnCollision(collision);

            HitCallback(collision.collider);
        }

        public delegate void ColliderDelegate(Collider2D collider);
        public event ColliderDelegate OnTrigger;
        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (OnTrigger != null) OnTrigger(collider);

            HitCallback(collider);
        }

        public event ColliderDelegate OnHit;
        protected virtual void HitCallback(Collider2D collider)
        {
            if (OnHit != null) OnHit(collider);
        }

        public event Action DestroyEvent;
        public virtual void Destroy()
        {
            if (DestroyEvent != null) DestroyEvent();

            Destroy(gameObject);
        }

        //Utility
        public static GameObject GetTarget(Collider2D collider)
        {
            return collider.attachedRigidbody == null ? collider.gameObject : collider.attachedRigidbody.gameObject;
        }
    }
}
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword