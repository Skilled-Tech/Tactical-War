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

        public override void Activate(Proponent proponent)
        {
            base.Activate(proponent);

            StartCoroutine(Procedure());
        }

        IEnumerator Procedure()
        {
            var instruction = new WaitForSeconds(duration / count);

            for (int i = 0; i < count; i++)
            {
                Spawn();

                yield return instruction;
            }

            End();
        }

        protected virtual void Spawn()
        {
            var instance = Instantiate(prefab);

            var script = instance.GetComponent<Projectile>();

            script.Configure(User.Base);

            script.SetLayer(User.Layer);

            instance.transform.position = transform.position + (transform.right * Random.Range(-range / 2f, range / 2f));
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