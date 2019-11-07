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
    [RequireComponent(typeof(SFXPlayer))]
    public class UnitAudio : Unit.Module
    {
        public AudioSource Component { get; protected set; }

        public SFXPlayer SFX { get; protected set; }

        public override void Configure(Unit reference)
        {
            base.Configure(reference);

            Component = Dependancy.Get<AudioSource>(gameObject);

            SFX = Dependancy.Get<SFXPlayer>(gameObject);
        }

        public override void Init()
        {
            base.Init();

            Unit.OnDeath += DeathCallback;
        }

        private void DeathCallback(Damage.Result result)
        {
            transform.SetParent(null);

            Destroy(gameObject, 2f);
        }
    }
}