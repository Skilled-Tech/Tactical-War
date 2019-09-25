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
	public class ProjectileTravelDistance : Projectile.Module
	{
        [SerializeField]
        protected float target;
        public float Target
        {
            get
            {
                return target;
            }
            set
            {
                this.target = value;
            }
        }

        [SerializeField]
        protected bool resetOnHit = true;
        public bool ResetOnHit { get { return resetOnHit; } }

        public float Value { get; protected set; }

        public override void Configure(Projectile data)
        {
            base.Configure(data);

            Value = 0f;
        }

        public override void Init()
        {
            base.Init();

            Projectile.OnHit += OnHit;
        }

        void Update()
        {
            Value += Projectile.rigidbody.velocity.magnitude * Time.deltaTime;

            if (Value >= target)
                Projectile.Destroy();
        }

        void OnHit(Collider2D collider)
        {
            Value = 0f;
        }
    }
}