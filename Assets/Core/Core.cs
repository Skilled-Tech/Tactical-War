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
    [CreateAssetMenu(menuName = MenuPath + "Asset")]
	public class Core : ScriptableObject
	{
        public const string MenuPath = "Core/";

        static Core _instance;
        public static Core Instance
        {
            get
            {
                if (_instance == null)
                {
                    var list = Resources.LoadAll<Core>("");

                    if (list.Length > 0)
                        _instance = list.First();
                }

                return _instance;
            }
        }

        #region Units
        [SerializeField]
        protected PlayerCore player;
        public PlayerCore Player { get { return player; } }

        [SerializeField]
        protected UnitsCore units;
        public UnitsCore Units { get { return units; } }

        public class Module : ScriptableObject
        {
            public const string MenuPath = Core.MenuPath + "Modules/";

            public virtual void Configure()
            {

            }

            public virtual void Init()
            {

            }
        }

        public virtual void ForAllModules(Action<Module> action)
        {
            action(player);
            action(units);
        }
        #endregion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnLoad()
        {
            if (Instance == null)
                throw new ArgumentException("No Core Asset Found");

            Instance.Configure();
        }

        void Configure()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            ForAllModules(ConfigureModule);
        }

        void ConfigureModule(Module module)
        {
            module.Configure();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Init();
        }

        void Init()
        {
            ForAllModules(InitModule);
        }
        void InitModule(Module module)
        {
            module.Init();
        }
    }
}