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
	public class UnitProjectileAttack : UnitAttack
	{
        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        [SerializeField]
        protected Transform point;
        public Transform Point { get { return point; } } 

        [SerializeField]
        protected float velocity = 20;
        public float Velocity { get { return velocity; } }

        protected override void Initiate()
        {
            base.Initiate();

            Unit.Body.CharacterAnimation.Attack();
        }

        protected override void Connected()
        {
            base.Connected();

            var instance = Create();
        }

        Projectile Create()
        {
            var instance = Instantiate(prefab, point.position, point.rotation);

            instance.name = "(" + Unit.name + ") " + prefab.name;

            var projectile = instance.GetComponent<Projectile>();

            projectile.Configure(Unit);

            projectile.AmmendLayer(Unit.Leader.Layer);
            projectile.SetVelocity(velocity);

            projectile.OnHit += OnProjectileHit;

            var travelDistance = Dependancy.Get<ProjectileTravelDistance>(projectile.gameObject);
            if(travelDistance != null)
            {
                travelDistance.Range = Attack.Distance * 2f;
            }

            var penetration = Dependancy.Get<ProjectilePenetration>(projectile.gameObject);
            if(penetration != null)
            {
                penetration.Value = Attack.Range.Value - 1 - Unit.Index;
            }

            return projectile;
        }

        void OnProjectileHit(Collider2D collider)
        {
            var entity = Projectile.GetTarget(collider).GetComponent<Entity>();

            if (entity == null)
            {

            }
            else
            {
                DoDamage(entity);
            }
        }
    }
}