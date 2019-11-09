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
    [Serializable]
	public class AudioCore : Core.Property
	{
        public GameObject GameObject { get; protected set; }

        public SFXPlayer SFX { get; protected set; }

        public MusicPlayer Music { get; protected set; }

        public override void Configure()
        {
            base.Configure();

            GameObject = new GameObject("Audio");
            GameObject.transform.SetParent(Core.SceneAcessor.transform);

            ConfigureSFX();
            ConfigureMusic();
        }

        protected virtual void ConfigureSFX()
        {
            var GameObject = new GameObject("SFX");

            GameObject.transform.SetParent(this.GameObject.transform);

            GameObject.AddComponent<AudioSource>();

            SFX = GameObject.AddComponent<SFXPlayer>();

            SFX.IgnoreListenerPause = true;
        }
        protected virtual void ConfigureMusic()
        {
            var GameObject = new GameObject("Music");

            GameObject.transform.SetParent(this.GameObject.transform);

            GameObject.AddComponent<AudioSource>();

            Music = GameObject.AddComponent<MusicPlayer>();
        }
    }
}