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

        #region Modules
        [SerializeField]
        protected DataCore data;
        public DataCore Data { get { return data; } }

        [SerializeField]
        protected ScenesCore scenes;
        public ScenesCore Scenes { get { return scenes; } }

        [SerializeField]
        protected LevelsCore levels;
        public LevelsCore Levels { get { return levels; } }

        [SerializeField]
        protected PlayerCore player;
        public PlayerCore Player { get { return player; } }

        [SerializeField]
        protected ItemsCore items;
        public ItemsCore Items { get { return items; } }

        [SerializeField]
        protected PlayFabCore playFab;
        public PlayFabCore PlayFab { get { return playFab; } }

        [Serializable]
        public class Module
        {
            public Core Core { get { return Core.Instance; } }

            public virtual void Configure()
            {

            }

            public virtual void Init()
            {

            }
        }

        public virtual void ForAllModules(Action<Module> action)
        {
            action(data);
            action(scenes);
            action(levels);
            action(items);
            action(player);
            action(playFab);
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

            Application.runInBackground = true;
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

        public static void Quit()
        {
            if(Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }
        }
    }
}