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

using UnityEngine.Audio;

namespace Game
{
    [Serializable]
	public class AudioCore : Core.Property
	{
        public GameObject GameObject { get; protected set; }

        [SerializeField]
        protected AudioMixer mixer;
        public AudioMixer Mixer { get { return mixer; } }

        [SerializeField]
        protected ChannelsCore channels;
        public ChannelsCore Channels { get { return channels; } }
        [Serializable]
        public class ChannelsCore : Property
        {
            [SerializeField]
            protected Channel[] list;
            public Channel[] List { get { return list; } }

            public int Count => list.Length;
            public Channel this[int index] => list[index];

            public virtual Channel Find(AudioMixerGroup mixerGroup)
            {
                for (int i = 0; i < Count; i++)
                    if (this[i].MixerGroup == mixerGroup)
                        return this[i];

                return null;
            }

            [SerializeField]
            protected CommonData common;
            public CommonData Common { get { return common; } }
            [Serializable]
            public class CommonData
            {
                [SerializeField]
                protected AudioMixerGroup master;
                public AudioMixerGroup Master { get { return master; } }

                [SerializeField]
                protected AudioMixerGroup _SFX;
                public AudioMixerGroup SFX { get { return _SFX; } }

                [SerializeField]
                protected AudioMixerGroup music;
                public AudioMixerGroup Music { get { return music; } }
            }

            public override void Configure()
            {
                base.Configure();

                for (int i = 0; i < Count; i++)
                {
                    Register(this[i]);
                }
            }
        }

        [Serializable]
        public class Channel : AudioCore.Property
        {
            [SerializeField]
            protected AudioMixerGroup mixerGroup;
            public AudioMixerGroup MixerGroup { get { return mixerGroup; } }

            public float Volume
            {
                get
                {
                    Audio.mixer.GetFloat(VolumeID, out var value);

                    value = Tools.Audio.DecibelToLinear(value);

                    return value;
                }
                set
                {
                    value = Mathf.Clamp01(value);

                    Audio.mixer.SetFloat(VolumeID, Tools.Audio.LinearToDecibel(value));

                    Core.Prefs.Set(VolumeID, value);
                }
            }
            public string VolumeID => mixerGroup.name + " " + nameof(Volume);

            public override void Configure()
            {
                base.Configure();

                IEnumerator Procedure()
                {
                    //Unity's Audio Mixer Doesn't get initialized so we have to wait one frame before using it
                    yield return new WaitForEndOfFrame();

                    Volume = Core.Prefs.GetFloat(VolumeID, Volume);
                }

                Core.SceneAcessor.StartCoroutine(Procedure());
            }
        }

        [SerializeField]
        protected PlayerProperty player;
        public PlayerProperty Player { get { return player; } }
        [Serializable]
        public class PlayerProperty : Property
        {
            public SFXPlayer SFX { get; protected set; }

            public MusicPlayer Music { get; protected set; }

            public override void Configure()
            {
                base.Configure();

                SFX = Create<SFXPlayer>(Audio.Channels.Common.SFX);
                SFX.IgnoreListenerPause = true;

                Music = Create<MusicPlayer>(Audio.Channels.Common.Music);
            }

            public virtual TPlayer Create<TPlayer>(AudioMixerGroup mixerGroup)
                where TPlayer : AudioPlayer
            {
                var gameObject = new GameObject(mixerGroup.name);

                gameObject.transform.SetParent(Audio.GameObject.transform);

                var component = gameObject.AddComponent<TPlayer>();

                component.MixerGroup = mixerGroup;

                return component;
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

            Register(Channels);
            Register(player);
        }
    }
}