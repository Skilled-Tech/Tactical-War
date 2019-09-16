using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using PlayFab;

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
        protected UICore _UI;
        public UICore UI { get { return _UI; } }

        [SerializeField]
        protected ScenesCore scenes;
        public ScenesCore Scenes { get { return scenes; } }

        [SerializeField]
        protected RegionsCore regions;
        public RegionsCore Regions { get { return regions; } }

        [SerializeField]
        protected PlayerCore player;
        public PlayerCore Player { get { return player; } }

        [SerializeField]
        protected ItemsCore items;
        public ItemsCore Items { get { return items; } }

        [SerializeField]
        protected PlayFabCore playFab;
        public PlayFabCore PlayFab { get { return playFab; } }

        public interface IModule
        {
            void Configure();

            void Init();
        }

        [Serializable]
        public class Module : IModule
        {
            public Core Core { get { return Core.Instance; } }

            public virtual void Configure()
            {

            }

            public event Action OnInit;
            public virtual void Init()
            {
                if (OnInit != null) OnInit();
            }

            public virtual void Register(IModule module)
            {
                module.Configure();

                OnInit += module.Init;
            }
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

            Application.runInBackground = true;

            Register(data);
            Register(UI);
            Register(scenes);
            Register(regions);
            Register(items);
            Register(player);
            Register(playFab);
        }

        public virtual void Register(Module module)
        {
            module.Configure();

            OnInit += module.Init;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Init();
        }

        public event Action OnInit;
        void Init()
        {
            if (OnInit != null) OnInit();
        }

        public static void Quit()
        {
            if (Application.isEditor)
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