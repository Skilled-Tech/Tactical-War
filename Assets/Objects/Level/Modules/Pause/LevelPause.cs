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
    public class LevelPause : MonoBehaviour, IReference<Level>
    {
        [SerializeField]
        protected LevelPauseState _state;
        public LevelPauseState State
        {
            get
            {
                return _state;
            }
            set
            {
                StateChanged(value);
            }
        }

        Level level;
        public void Init(Level reference)
        {
            level = reference;
        }

        public event Action<LevelPauseState> OnStateChanged;
        protected virtual void StateChanged(LevelPauseState target)
        {
            _state = target;

            if (State == LevelPauseState.Hard)
                Time.timeScale = 1f;
            else
                Time.timeScale = 0f;

            if (OnStateChanged != null) OnStateChanged(_state);
        }
    }

    public enum LevelPauseState
    {
        None, Soft, Hard
    }
}