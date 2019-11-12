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

        [SerializeField]
        protected SFXCore _SFX;
        public SFXCore SFX { get { return _SFX; } }
        [Serializable]
        public class SFXCore : PlayerProperty<SFXPlayer>
        {
            public override string Name => "SFX";
        }

        [SerializeField]
        protected MusicCore music;
        public MusicCore Music { get { return music; } }
        [Serializable]
        public class MusicCore : PlayerProperty<MusicPlayer>
        {
            public override string Name => "Music";
        }

        [Serializable]
        public abstract class PlayerProperty<TComponent> : ProceduralProperty<TComponent>
            where TComponent : AudioPlayer
        {
            public TComponent Player => Component;
        }

        [Serializable]
        public abstract class ProceduralProperty<TComponent> : Property
            where TComponent: Component
        {
            public GameObject GameObject { get; protected set; }

            public TComponent Component { get; protected set; }

            public abstract string Name { get; }

            public override void Configure()
            {
                base.Configure();

                Create();
            }

            protected virtual void Create()
            {
                GameObject = new GameObject(Name);

                GameObject.transform.SetParent(Audio.GameObject.transform);

                Component = GameObject.AddComponent<TComponent>();
            }
        }

        [Serializable]
        public abstract class Property : Core.Property
        {
            public AudioCore Audio => Core.Audio;
        }

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