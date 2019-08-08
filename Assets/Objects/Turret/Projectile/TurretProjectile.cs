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
	public class TurretProjectile : MonoBehaviour
	{
        [SerializeField]
        protected float damage;
        public float Damage { get { return damage; } } 

        new public Rigidbody2D rigidbody { get; protected set; }
        public void AddForce(float force)
        {
            rigidbody.velocity = transform.right * force;
        }

        Turret turret;
        public virtual void Init(Turret turret)
        {
            this.turret = turret;

            rigidbody = GetComponent<Rigidbody2D>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var unit = collision.gameObject.GetComponent<Unit>();

            if (unit != null && unit.Leader != turret.Proponent)
            {
                turret.Proponent.Base.DoDamage(unit, damage);
            }

            Destroy(gameObject);
        }
    }
}