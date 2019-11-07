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
    [CreateAssetMenu(menuName = Core.GameMenuPath + "Music Track")]
	public class MusicTrack : ScriptableObject
	{
        [SerializeField]
        protected AudioClip[] clips;
        public AudioClip[] Clips { get { return clips; } }

        public int Size => clips.Length;
        public AudioClip this[int index] => clips[index];

        public AudioClip First => this[0];
        public AudioClip Last => clips[Size - 1];
    }
}