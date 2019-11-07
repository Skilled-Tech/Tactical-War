﻿using System;
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
	public class AudioPlayer : MonoBehaviour
	{
        public AudioSource Source { get; protected set; }

        public bool IsPlaying => Source.isPlaying;

        public float Volume { get => Source.volume; set => Source.volume = value; }

        public float Pitch { get => Source.pitch; set => Source.pitch = value; }

        protected virtual void Start()
        {
            Source = GetComponent<AudioSource>();
        }
    }
}