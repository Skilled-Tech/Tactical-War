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
    [RequireComponent(typeof(SFXPlayer))]
	public class ProjectileSFX : Projectile.ActivationModule
	{
        [SerializeField]
        protected SFXProperty[] _SFX;
        public SFXProperty[] SFX { get { return _SFX; } }

        SFXPlayer player;

        public override void Configure(Projectile reference)
        {
            base.Configure(reference);

            player = GetComponent<SFXPlayer>();
        }

        protected override void Process()
        {
            if(Projectile.Armed)
                player.PlayOneShot(SFX);
        }
    }
}