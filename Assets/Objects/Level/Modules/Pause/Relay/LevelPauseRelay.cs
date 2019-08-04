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
	public class LevelPauseRelay : Relay
	{
        public LevelPauseState condition;

        public LevelPause Pause { get { return Level.Instance.Pause; } }

        private void Start()
        {
            Pause.OnStateChanged += OnStateChagned;
        }

        private void OnStateChagned(LevelPauseState state)
        {
            if (Pause.State == condition)
                Invoke();
        }
    }
}