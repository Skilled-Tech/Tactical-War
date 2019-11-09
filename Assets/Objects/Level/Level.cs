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

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
namespace Game
{
    [DefaultExecutionOrder(ExecutionOrder)]
    public class Level : MonoBehaviour
    {
        public const int ExecutionOrder = -200;

        public const string MenuPath = Core.GameMenuPath + "Level/";

        public static Level Instance { get; protected set; }

        public GameCamera camera { get; protected set; }

        public LevelPause Pause { get; protected set; }
        public LevelProponents Proponents { get; protected set; }
        public LevelSpeed Speed { get; protected set; }
        public LevelMenu Menu { get; protected set; }
        public LevelBackground Background { get; protected set; }
        public LevelFinish Finish { get; protected set; }
        public LevelTimer Timer { get; protected set; }

        public static Core Core { get { return Core.Instance; } }
        public static WorldCore World { get { return Core.World; } }
        public static WorldCore.CurrentData Data { get { return World.Current; } }

        public abstract class Module : Module<Level>
        {
            public Level Level { get { return Reference; } }

            public Core Core { get { return Core.Instance; } }
            public WorldCore World { get { return Core.World; } }
            public WorldCore.CurrentData Data { get { return World.Current; } }
        }

        protected virtual void Awake()
        {
            Core.PlayFab.EnsureActivation();

            Instance = this;

            Pause = Dependancy.Get<LevelPause>(gameObject);
            Speed = Dependancy.Get<LevelSpeed>(gameObject);
            Proponents = Dependancy.Get<LevelProponents>(gameObject);

            Menu = FindObjectOfType<LevelMenu>();
            Menu.Configure(this);

            Background = Dependancy.Get<LevelBackground>(gameObject);
            Finish = Dependancy.Get<LevelFinish>(gameObject);
            Timer = Dependancy.Get<LevelTimer>(gameObject);

            camera = FindObjectOfType<GameCamera>();

            Modules.Configure(this);
        }

        protected virtual void Start()
        {
            Modules.Init(this);

            Menu.Init();

            Proponents.OnDefeat += OnProponentDeafeated;

            Core.Audio.Music.Play(Data.Level.Music);
        }

        void OnProponentDeafeated(Proponent proponent)
        {
            Finish.Process(Proponents.GetOther(proponent));
        }

        protected virtual void Exit()
        {
            Pause.State = LevelPauseState.Soft;

            Core.Audio.Music.FadeOut();
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }

        //Utility
        public static void Quit()
        {
            Instance.Exit();

            Core.Scenes.Load.One(Core.Scenes.MainMenu);
        }

        public static void Retry()
        {
            Instance.Exit();

            Data.Level.Reload();
        }

        public static void Next()
        {
            if (Data.Level.IsLast)
                throw new InvalidOperationException(Data.Level.name + " is the last Level in the " + Data.Region.name + ", Can't Progress any further");

            Instance.Exit();

            var target = Data.Level.Next;

            target.Load(Data.Difficulty);
        }
    }
}
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword