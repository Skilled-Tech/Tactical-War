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
    public class ProjectileExplosion : Projectile.Module
    {
        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        [SerializeField]
        protected float radius;
        public float Radius { get { return radius; } }

        public override void Init()
        {
            base.Init();

            Projectile.OnCollision += OnCollision;
        }

        void OnCollision(Collision2D obj)
        {
            Instantiate(prefab, transform.position, transform.rotation);

            var damage = Dependancy.Get<ProjectileDamage>(Projectile.gameObject);

            if(damage != null)
            {
                var colliders = Physics2D.OverlapCircleAll(transform.position, radius);

                for (int i = 0; i < colliders.Length; i++)
                {
                    var entity = colliders[i].GetComponent<Entity>();

                    if (entity == null) continue;
                    if (entity is Base) continue;
                    if (entity.gameObject.layer == gameObject.layer) continue;

                    Owner.DoDamage(entity, damage.Value);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}