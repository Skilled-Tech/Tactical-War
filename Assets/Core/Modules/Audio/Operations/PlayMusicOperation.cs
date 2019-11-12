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
	public class PlayMusicOperation : Operation
	{
        [SerializeField]
        protected MusicTrack track;
        public MusicTrack Track { get { return track; } }

        public Core Core => Core.Instance;

        public override void Execute()
        {
            Core.Audio.Player.Music.Play(track);
        }
    }
}