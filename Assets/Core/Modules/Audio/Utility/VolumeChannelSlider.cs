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
    [RequireComponent(typeof(Slider))]
	public class VolumeChannelSlider : MonoBehaviour
	{
        [SerializeField]
        protected AudioMixerGroup mixerGroup;
        public AudioMixerGroup MixerGroup { get { return mixerGroup; } }

        public Slider Slider { get; protected set; }

        public AudioCore.Channel Channel { get; protected set; }

        public AudioCore AudioCore => Core.Instance.Audio;

        private void Start()
        {
            Channel = AudioCore.Channels.Find(mixerGroup);

            Slider = GetComponent<Slider>();

            Slider.minValue = 0f;
            Slider.maxValue = 1f;

            Slider.value = Channel.Volume;

            Slider.onValueChanged.AddListener(ChangeCallback);
        }

        void ChangeCallback(float newValue)
        {
            Channel.Volume = Slider.value;
        }
    }
}