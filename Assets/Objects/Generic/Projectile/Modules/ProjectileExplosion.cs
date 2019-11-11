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
    public class ProjectileExplosion : Projectile.ActivationModule
    {
        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        [SerializeField]
        protected float radius;
        public float Radius { get { return radius; } }

        protected override void Process()
        {
            var instance = Instantiate(prefab, transform.position, transform.rotation);

            instance.transform.SetParent(Projectile.transform.parent);

            var damage = Dependancy.Get<ProjectileDamage>(Projectile.gameObject);

            if (damage != null)
            {
                var colliders = Physics2D.OverlapCircleAll(transform.position, radius);

                for (int i = 0; i < colliders.Length; i++)
                {
                    var target = colliders[i].GetComponent<Entity>();

                    if (target == null) continue;
                    if (target.gameObject.layer == gameObject.layer) continue;

                    Projectile.DoDamage(damage.Value, Damage.Method.Effect, target);
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