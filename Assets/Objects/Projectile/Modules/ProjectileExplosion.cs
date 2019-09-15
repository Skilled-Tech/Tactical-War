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
    public class ProjectileExplosion : Projectile.Module
    {
        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        public override void Init()
        {
            base.Init();

            Projectile.OnCollision += OnCollision;
        }

        void OnCollision(Collision2D obj)
        {
            Instantiate(prefab, transform.position, transform.rotation);
        }
    }
}