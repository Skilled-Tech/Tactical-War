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
        protected Transform spawn;
        public Transform Spawn { get { return spawn; } } 

        [SerializeField]
        protected float velocity = 20;
        public float Velocity { get { return velocity; } } 

        public override void AttackConnected()
        {
            base.AttackConnected();

            var instance = Get();
        }

        Projectile Get()
        {
            var instance = Instantiate(prefab, spawn.position, spawn.rotation);

            var projectile = instance.GetComponent<Projectile>();

            projectile.Configure(Unit);

            projectile.AmmendLayer(Unit.Leader.Layer);
            projectile.SetVelocity(velocity);

            projectile.GetComponent<ProjectileDamage>().Value = Damage;

            return projectile;
        }
    }
}