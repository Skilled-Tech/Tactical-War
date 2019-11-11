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
	public class ProjectileDamage : Projectile.Module
	{
		[SerializeField]
        protected float value;
        public float Value
        {
            get
            {
                return value;
            }
            set
            {
                if (value < 0f) value = 0f;

                this.value = value;
            }
        }

        public override void Init()
        {
            base.Init();

            Projectile.OnHit += OnHit;
        }

        void OnHit(Collider2D collider)
        {
            var entity = collider.gameObject.GetComponent<Entity>();

            if (entity == null)
            {

            }
            else
            {
                Projectile.DoDamage(Value, Damage.Method.Ranged, entity);
            }
        }
    }
}