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
	public class MeteorShowerAbility : Ability
	{
		[SerializeField]
        protected float range;
        public float Range { get { return range; } }

        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        [SerializeField]
        protected float duration = 10f;
        public float Duration { get { return duration; } } 

        [SerializeField]
        protected int count = 20;
        public int Count { get { return count; } }

        public List<Projectile> Elements { get; protected set; }

        public override void Configure(Proponent user, AbilityTemplate template)
        {
            base.Configure(user, template);

            Elements = new List<Projectile>();
        }

        public override void Init()
        {
            base.Init();

            StartCoroutine(Procedure());
        }

        IEnumerator Procedure()
        {
            var instruction = new WaitForSeconds(duration / count);

            for (int i = 0; i < count; i++)
            {
                var instance = Spawn();

                instance.DestroyEvent += ()=> ProjectileDestroyCallback(instance);

                Elements.Add(instance);

                yield return instruction;
            }

            bool HasElements() => Elements.Count > 0;
            yield return new WaitWhile(HasElements);

            End();
        }

        void ProjectileDestroyCallback(Projectile instance)
        {
            Elements.Remove(instance);
        }

        protected virtual Projectile Spawn()
        {
            var instance = Instantiate(prefab);

            instance.transform.SetParent(transform);

            var script = instance.GetComponent<Projectile>();

            script.Configure(User.Base);
            script.SetLayer(User.Layer);

            instance.transform.position = transform.position + (transform.right * Random.Range(-range / 2f, range / 2f));
            instance.transform.rotation = transform.rotation;

            return script;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawLine(Vector3.right * range / 2f, Vector3.left * range / 2f);

            Gizmos.DrawSphere(Vector3.right * range / 2f, 0.25f);
            Gizmos.DrawSphere(Vector3.left * range / 2f, 0.25f);
        }
    }
}