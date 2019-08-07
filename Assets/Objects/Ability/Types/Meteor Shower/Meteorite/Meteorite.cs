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
    [RequireComponent(typeof(Rigidbody))]
	public class Meteorite : MonoBehaviour
	{
		[SerializeField]
        protected float damage = 120;
        public float Damage { get { return damage; } }

        [SerializeField]
        protected Vector3 velocity;
        public Vector3 Velocity { get { return velocity; } } 

        new Rigidbody rigidbody;

        MeteorShowerAbility ability;
        public virtual void Init(MeteorShowerAbility ability)
        {
            this.ability = ability;

            rigidbody = GetComponent<Rigidbody>();
            rigidbody.velocity = velocity;
        }

        void OnCollisionEnter(Collision collision)
        {
            var unit = collision.gameObject.GetComponent<Unit>();

            if(unit != null && unit.Leader != ability.User)
            {
                ability.User.Base.DoDamage(unit, damage);
            }

            Destroy(gameObject);
        }
    }
}