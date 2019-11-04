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
	public class ProjectileAudio : Projectile.ActivationModule
	{
        [SerializeField]
        protected AudioClip clip;
        public AudioClip Clip { get { return clip; } }

        public AudioSource Source { get; protected set; }

        public override void Configure(Projectile reference)
        {
            base.Configure(reference);

            Source = Dependancy.Get<AudioSource>(gameObject);

            if (Source == null)
                Source = Dependancy.Get<AudioSource>(Projectile.gameObject);
        }

        public override void Init()
        {
            base.Init();

            Projectile.DestroyEvent += DestroyCallback;
        }

        protected override void DestroyCallback()
        {
            base.DestroyCallback();

            transform.SetParent(null);
            Destroy(gameObject, 2);
        }

        protected override void Process()
        {
            Source.PlayOneShot(clip);
        }
    }
}