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
	public class ProjectileTimer : Projectile.Module
	{
        [SerializeField]
        protected float delay = 5f;
        public float Delay { get { return delay; } }

        public override void Init()
        {
            base.Init();

            StartCoroutine(Procedure());
        }

        IEnumerator Procedure()
        {
            yield return new WaitForSeconds(delay);

            Projectile.Destroy();
        }
	}
}