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
	public class ProjectileVelocity : Projectile.Module
	{
        [SerializeField]
        protected Vector2 vector;
        public Vector2 Vector { get { return vector; } }

        public Space space = Space.World;

        public override void Init()
        {
            base.Init();

            Projectile.rigidbody.gravityScale = 0;

            Projectile.rigidbody.velocity = Calculate();
        }

        public Vector3 Calculate()
        {
            switch (space)
            {
                case Space.World:
                    return vector;

                case Space.Self:
                    return transform.InverseTransformVector(vector);
            }

            return vector;
        }
    }
}