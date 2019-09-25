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
	public class ProjectilePenetration : Projectile.Module
	{
        [SerializeField]
        protected uint value = 1;
        public uint Value { get { return value; } }

        uint count;

        public override void Configure(Projectile data)
        {
            base.Configure(data);

            count = 0;
        }

        public override void Init()
        {
            base.Init();

            Projectile.OnHit += HitCallback;
        }

        void HitCallback(Collider2D collider)
        {
            count++;

            if (count >= value)
                Projectile.Destroy();
        }
    }
}