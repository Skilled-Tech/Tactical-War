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
    [DefaultExecutionOrder(ExecutionOrder)]
	public class Level : MonoBehaviour
	{
        public const int ExecutionOrder = -200;

        public static Level Instance { get; protected set; }

        new public GameCamera camera { get; protected set; }

        public LevelPause Pause { get; protected set; }
        public LevelProponents Proponents { get; protected set; }
        public LevelSpeed Speed { get; protected set; }
        public LevelMenu Menu { get; protected set; }

        public Core Core { get { return Core.Instance; } }

        public abstract class Module : Module<Level>
        {
            public Level Level { get { return Reference; } }
        }

        protected virtual void Awake()
        {
            Instance = this;

            Modules.Configure(this);

            Pause = Dependancy.Get<LevelPause>(gameObject);
            Speed = Dependancy.Get<LevelSpeed>(gameObject);
            Proponents = Dependancy.Get<LevelProponents>(gameObject);

            Menu = FindObjectOfType<LevelMenu>();
            Menu.Configure(this);

            camera = FindObjectOfType<GameCamera>();
        }

		protected virtual void Start()
        {
            Modules.Init(this);

            Menu.Init();

            Proponents.OnDefeat += OnProponentDeafeated;
        }

        void OnProponentDeafeated(Proponent proponent)
        {
            Speed.Value = 0f;

            Menu.End.Show(Proponents.GetOther(proponent));
        }

        public virtual void Quit()
        {
            Speed.Value = 1f;

            Core.Levels.ReturnToMainMenu();
        }

        public virtual void Retry()
        {
            Speed.Value = 1f;

            Core.Instance.Levels.Reload();
        }

        protected virtual void OnDestroy()
        {
            Instance = null;
        }
	}
}