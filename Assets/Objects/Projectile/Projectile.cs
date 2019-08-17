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
    [RequireComponent(typeof(Rigidbody2D))]
	public class Projectile : MonoBehaviour
	{
        new public Rigidbody2D rigidbody { get; protected set; }
        public void SetVelocity(float velocity)
        {
            rigidbody.velocity = transform.right * velocity;
        }

        public class Module : Module<Projectile>
        {
            public Projectile Projectile { get { return Data; } }

            public Entity Owner { get { return Projectile.Owner; } }
        }

        public Entity Owner { get; protected set; }

        public virtual void Configure(Entity owner)
        {
            this.Owner = owner;

            rigidbody = GetComponent<Rigidbody2D>();

            Modules.Configure(this);
        }

        public virtual void SetLayer(int layer)
        {
            Tools.SetLayer(gameObject, layer);
        }

        private void Start()
        {
            Modules.Init(this);
        }

        public event Action<Collision2D> OnCollision;
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (OnCollision != null) OnCollision(collision);

            Destroy(gameObject);
        }
    }
}