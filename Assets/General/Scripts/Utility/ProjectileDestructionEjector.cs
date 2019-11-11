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
	public class ProjectileDestructionEjector : Projectile.Module
	{
        public override void Configure(Projectile reference)
        {
            base.Configure(reference);

            Projectile.DestroyEvent += DestroyCallback;
        }

        private void DestroyCallback()
        {
            transform.SetParent(null);

            IEnumerator Procedure()
            {
                yield return new WaitForSeconds(2f);

                Destroy(gameObject);
            }

            StartCoroutine(Procedure());
        }
    }
}