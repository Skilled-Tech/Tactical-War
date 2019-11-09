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

        public const string GameMenuPath = "Game/";

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

        public CoreSceneAccessor SceneAcessor { get; protected set; }

        #region Modules
        [SerializeField]
        protected DataCore data;
        public DataCore Data { get { return data; } }

        [SerializeField]
        protected PrefsCore prefs;
        public PrefsCore Prefs { get { return prefs; } }

        [SerializeField]
        protected UICore _UI;
        public UICore UI { get { return _UI; } }

        [SerializeField]
        protected ScenesCore scenes;
        public ScenesCore Scenes { get { return scenes; } }

        [SerializeField]
        protected WorldCore world;
        public WorldCore World { get { return world; } }

        [SerializeField]
        protected PlayerCore player;
        public PlayerCore Player { get { return player; } }

        [SerializeField]
        protected ShopCore shop;
        public ShopCore Shop { get { return shop; } }

        [SerializeField]
        protected ItemsCore items;
        public ItemsCore Items { get { return items; } }

        [SerializeField]
        protected PlayFabCore playFab;
        public PlayFabCore PlayFab { get { return playFab; } }

        [SerializeField]
        protected IAPCore _IAP;
        public IAPCore IAP { get { return _IAP; } }

        [SerializeField]
        protected LocalizationCore localization;
        public LocalizationCore Localization { get { return localization; } }

        [SerializeField]
        protected AudioCore audio;
        public AudioCore Audio { get { return audio; } }

        public interface IProperty
        {
            void Configure();

            event Action OnInit;
            void Init();

            void Register(IProperty property);
        }

        [Serializable]
        public class Property : IProperty
        {
            public static Core Core { get { return Core.Instance; } }

            public virtual void Configure()
            {

            }

            public event Action OnInit;
            public virtual void Init()
            {
                if (OnInit != null) OnInit();
            }

            public virtual void Register(IProperty module)
            {
                module.Configure();

                OnInit += module.Init;
            }
        }

        public class Module : ScriptableObject, IProperty
        {
            public const string MenuPath = Core.MenuPath + "Modules/";

            public static Core Core { get { return Core.Instance; } }

            public virtual void Configure()
            {

            }

            public event Action OnInit;
            public virtual void Init()
            {
                if (OnInit != null) OnInit();
            }

            public event Action OnSceneLoad;
            public virtual void SceneLoad()
            {
                if (OnSceneLoad != null) OnSceneLoad();
            }

            public virtual void Register(IProperty module)
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
            Application.runInBackground = true;

            SceneAcessor = CoreSceneAccessor.Create();

            Register(data);
            Register(prefs);
            Register(UI);
            Register(scenes);
            Register(world);
            Register(items);
            Register(shop);
            Register(playFab);
            Register(player);
            Register(IAP);
            Register(localization);
            Register(audio);

            Init();
        }

        public virtual void Register(IProperty module)
        {
            module.Configure();

            OnInit += module.Init;
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