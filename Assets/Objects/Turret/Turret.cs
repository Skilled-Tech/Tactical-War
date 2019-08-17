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
	public class Turret : MonoBehaviour
	{
        [SerializeField]
        protected float range;
        public float Range { get { return range; } } 

        [SerializeField]
        protected float aimSpeed = 200f;
        public float AimSpeed { get { return aimSpeed; } } 

        [SerializeField]
        protected float reloadTime = 0.5f;
        public float ReloadTime { get { return reloadTime; } } 

        [SerializeField]
        protected GameObject projectilePrefab;
        public GameObject ProjectilePrefab { get { return projectilePrefab; } } 

        [SerializeField]
        protected Transform projectileSpawn;
        public Transform ProjectileSpawn { get { return projectileSpawn; } } 

        [SerializeField]
        protected float velocity = 25;
        public float Velocity { get { return velocity; } } 

        public Base Base { get { return slot.Base; } }
        public Proponent Proponent { get { return Base.Proponent; } }

        public Proponent Enemy { get { return Proponent.Enemey; } }

        BaseTowerSlot slot;
		public virtual void Init(BaseTowerSlot slot)
        {
            this.slot = slot;
        }

        float timer = 0f;

        void Update()
        {
            if(timer == 0f)
            {
                if (Enemy.Units.Count > 0)
                {
                    var distance = Mathf.Abs(Enemy.Base.Units.List[0].transform.position.x - transform.position.x);

                    if (distance <= range)
                    {
                        if (AimAt(Enemy.Base.Units.List[0].transform.position) == 0f)
                        {
                            Shoot();

                            timer = reloadTime + Random.Range(0f, 1f);
                        }
                    }
                }
            }
            else
            {
                timer = Mathf.MoveTowards(timer, 0f, Time.deltaTime);
            }
        }

        void Shoot()
        {
            var instance = Instantiate(projectilePrefab);

            instance.transform.position = projectileSpawn.transform.position;
            instance.transform.rotation = projectileSpawn.transform.rotation;

            var projectile = instance.GetComponent<Projectile>();

            projectile.Configure(Base);

            projectile.SetLayer(Proponent.Layer);

            projectile.SetVelocity(velocity);
        }

        float AimAt(Vector3 target)
        {
            target += Vector3.up * 1f;

            var direction = (target - transform.position).normalized;

            var targetRotation = Quaternion.LookRotation(direction, Vector2.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, aimSpeed * Time.deltaTime);

            return Quaternion.Angle(transform.rotation, targetRotation);
        }
    }
}