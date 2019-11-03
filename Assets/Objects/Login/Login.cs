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

using PlayFab;
using PlayFab.ClientModels;

namespace Game
{
    public class Login : MonoBehaviour
    {
        public GoogleLogin Google { get; protected set; }

        public EmailLogin Email { get; protected set; }

        public GuestLogin Guest { get; protected set; }

        public OfflineLogin Offline { get; protected set; }

        public abstract class Module : UIElementModule<Login>
        {
            public Login Login { get { return Reference; } }

            #region Accessors
            public static Core Core { get { return Core.Instance; } }

            public static PopupUI Popup { get { return Core.UI.Popup; } }

            public static ScenesCore Scenes { get { return Core.Scenes; } }

            public static PlayerCore Player { get { return Core.Player; } }

            public static PlayFabCore PlayFab { get { return Core.PlayFab; } }

            public static LocalizationCore Localization => Core.Localization;
            #endregion
        }

        public abstract class Controller : Module
        {
            [SerializeField]
            protected Button entry;
            public Button Entry { get { return entry; } }

            public abstract bool Accessible { get; }

            public abstract bool Available { get; }

            public override void Init()
            {
                base.Init();

                UpdateState();
            }

            public virtual void Perform()
            {
                Login.OnError += ErrorCallback;

                Popup.Show(Localization.Phrases.Get("Logging In"));
            }

            protected virtual void UpdateState()
            {
                entry.interactable = Accessible;

                entry.gameObject.SetActive(Available);
            }

            void ErrorCallback(PlayFabError error)
            {
                Login.OnError -= ErrorCallback;

                ErrorProcedure(error);
            }

            protected virtual void ErrorProcedure(PlayFabError error)
            {
                Popup.Show(error.ErrorMessage, Login.Reload, "Reload");
            }
        }

        #region Accessors
        public static Core Core { get { return Core.Instance; } }

        public static PopupUI Popup { get { return Core.UI.Popup; } }

        public static LocalizationCore Localization => Core.Localization;

        public static ScenesCore Scenes { get { return Core.Scenes; } }

        public static PlayerCore Player { get { return Core.Player; } }

        public static PlayFabCore PlayFab { get { return Core.PlayFab; } }
        #endregion

        private void Awake()
        {
            Google = Dependancy.Get<GoogleLogin>(gameObject);
            Email = Dependancy.Get<EmailLogin>(gameObject);
            Guest = Dependancy.Get<GuestLogin>(gameObject);
            Offline = Dependancy.Get<OfflineLogin>(gameObject);

            Modules.Configure(this);
        }

        private void Start()
        {
            PlayFab.Login.OnResponse += LoginResponseCallback;

            Modules.Init(this);
        }

        public virtual void Reload()
        {
            Scenes.Load(Scenes.Login);

            Popup.Hide();
        }
        
        void LoginResponseCallback(LoginResult result, PlayFabError error)
        {
            PlayFab.Login.OnResponse -= LoginResponseCallback;

            if (error == null)
            {
                Popup.Show(Localization.Phrases.Get("Retrieving Title Data"));

                Core.Prefs.NeedOnlineLogin.Value = false;

                PlayFab.Title.OnResponse += TitleResponseCallback;
                PlayFab.Title.Request();
            }
            else
            {
                Error(error);
            }
        }

        void TitleResponseCallback(PlayFabTitleCore data, PlayFabError error)
        {
            PlayFab.Title.OnResponse -= TitleResponseCallback;

            if (error == null)
            {
                Popup.Show(Localization.Phrases.Get("Retrieving Catalog"));

                PlayFab.Catalog.OnResponse += CatalogResponseCallback;
                PlayFab.Catalog.Request();
            }
            else
            {
                Error(error);
            }
        }

        void CatalogResponseCallback(PlayFabCatalogCore catalog, PlayFabError error)
        {
            PlayFab.Catalog.OnResponse -= CatalogResponseCallback;

            if (error == null)
            {
                if(PlayFab.IsLoggedIn)
                {
                    Popup.Show(Localization.Phrases.Get("Retrieving Daily Reward"));

                    PlayFab.DailyReward.OnResponse += DailyRewardsResponseCallback;
                    PlayFab.DailyReward.Claim();
                }
                else
                {
                    RetrievePlayerData();
                }
            }
            else
            {
                Error(error);
            }
        }

        void RetrievePlayerData()
        {
            Popup.Show(Localization.Phrases.Get("Retrieving Player Data"));

            PlayFab.Player.OnResponse += PlayerResponseCallback;
            PlayFab.Player.Retrieve();
        }

        PlayFabDailyRewardCore.ResultData dailyReward;
        void DailyRewardsResponseCallback(PlayFabDailyRewardCore.ResultData result, PlayFabError error)
        {
            PlayFab.DailyReward.OnResponse -= DailyRewardsResponseCallback;

            if(error == null)
            {
                dailyReward = result;

                RetrievePlayerData();
            }
            else
            {
                Error(error);
            }
        }

        void PlayerResponseCallback(PlayFabPlayerCore result, PlayFabError error)
        {
            PlayFab.Player.OnResponse -= PlayerResponseCallback;

            if (error == null)
            {
                Finish();
            }
            else
            {
                Error(error);
            }
        }

        void Finish()
        {
            void Progress()
            {
                Core.UI.Rewards.OnFinish -= Progress;

                Popup.Hide();

                PlayFab.Activated = true;

                Scenes.Load(Scenes.MainMenu);
            }

            Popup.Hide();

            if(dailyReward == null || dailyReward.Items == null || dailyReward.Items.Length == 0)
            {
                Progress();
            }
            else
            {
                var stacks = ItemStack.From(dailyReward.Items);

                var title = Localization.Phrases.Get("daily reward") + Environment.NewLine;
                title += Localization.Phrases.Get("day") + " " + (dailyReward.Progress + 1).ToString() + "\\" + PlayFab.DailyReward.Max;

                Core.UI.Rewards.OnFinish += Progress;
                Core.UI.Rewards.Show(title, stacks);
            }
        }

        public delegate void ErrorDelegate(PlayFabError error);
        public event ErrorDelegate OnError;
        void Error(PlayFabError error)
        {
            Debug.LogError("Login Error: " + error.GenerateErrorReport());

            if (OnError != null) OnError(error);
        }
    }
}