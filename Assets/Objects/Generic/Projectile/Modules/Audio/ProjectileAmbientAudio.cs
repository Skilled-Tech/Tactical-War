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
    [RequireComponent(typeof(AudioSource))]
	public class ProjectileAmbientAudio : Projectile.Module
	{
        [SerializeField]
        protected AudioClip clip;
        public AudioClip Clip { get { return clip; } }

        AudioSource source;

        public override void Configure(Projectile reference)
        {
            base.Configure(reference);

            source = GetComponent<AudioSource>();

            source.loop = true;

            Projectile.OnArm += ArmCallback;
            Projectile.OnDisarm += DisarmCallback;

            Projectile.DestroyEvent += DestroyEvent;
        }

        void ArmCallback()
        {
            source.clip = clip;
            source.Play();
        }

        void DisarmCallback()
        {
            source.Stop();
        }

        void DestroyEvent()
        {
            source.Stop();
        }
    }
}