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
    public class LevelPause : Level.Module
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

        public LevelSpeed Speed { get { return Level.Speed; } }

        public event Action<LevelPauseState> OnStateChanged;
        protected virtual void StateChanged(LevelPauseState target)
        {
            _state = target;

            if (State == LevelPauseState.Hard)
                Time.timeScale = 0f;
            else
                Time.timeScale = Speed.Value;

            if(State == LevelPauseState.None)
                UnitBody.ToggleOff();

            if (OnStateChanged != null) OnStateChanged(_state);
        }
    }

    public enum LevelPauseState
    {
        None, Soft, Hard
    }
}