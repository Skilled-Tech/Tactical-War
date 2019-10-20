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
	public class UnitProjectileAttack : UnitAttack.Module
	{
        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        [SerializeField]
        protected Transform spawn;
        public Transform Spawn { get { return spawn; } } 

        [SerializeField]
        protected float velocity = 20;
        public float Velocity { get { return velocity; } }

        protected override void OnInitiated()
        {
            base.OnInitiated();

            Unit.Body.CharacterAnimation.Attack();
        }

        protected override void OnConnected()
        {
            base.OnConnected();

            var instance = Get();
        }

        Projectile Get()
        {
            var instance = Instantiate(prefab, spawn.position, spawn.rotation);

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
                penetration.Value = Attack.Range.Value - Unit.Index - 1;
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