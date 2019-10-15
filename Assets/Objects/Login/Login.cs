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

        #region Accessors
        public static Core Core { get { return Core.Instance; } }

        public static PopupUI Popup { get { return Core.UI.Popup; } }

        public static ScenesCore Scenes { get { return Core.Scenes; } }

        public static PlayerCore Player { get { return Core.Player; } }

        public static PlayFabCore PlayFab { get { return Core.PlayFab; } }
        #endregion

        public class Module : Module<Login>
        {
            public Login Login { get { return Reference; } }

            #region Accessors
            public static Core Core { get { return Core.Instance; } }

            public static PopupUI Popup { get { return Core.UI.Popup; } }

            public static ScenesCore Scenes { get { return Core.Scenes; } }

            public static PlayerCore Player { get { return Core.Player; } }

            public static PlayFabCore PlayFab { get { return Core.PlayFab; } }
            #endregion

            public virtual void Execute()
            {

            }
        }

        private void Awake()
        {
            Google = Dependancy.Get<GoogleLogin>(gameObject);
            Email = Dependancy.Get<EmailLogin>(gameObject);

            Modules.Configure(this);
        }

        private void Start()
        {
            Popup.Show("Logging In", null, null);

            if (PlayFab.startOffline && Application.isEditor)
            {
                StartOffline();

                PlayFab.startOffline = false;
            }
            else if(Google.Force && Application.isEditor)
            {
                Google.Execute();
            }
            else
            {
                PlayFab.Login.OnResponse += LoginResponseCallback;

                switch(Application.platform)
                {
                    case RuntimePlatform.Android:
                        Google.Execute();
                        break;

                    default:
                        Email.Execute();
                        break;
                }
            }

            Modules.Init(this);
        }

        public virtual void Retry()
        {
            Scenes.Load(Scenes.Login);
        }

        public virtual void StartOffline()
        {
            PlayFab.Title.OnResponse += TitleResponseCallback;
            PlayFab.Title.Request();
        }

        void LoginResponseCallback(LoginResult result, PlayFabError error)
        {
            PlayFab.Login.OnResponse -= LoginResponseCallback;

            if (error == null)
            {
                Popup.Show("Retrieving Title Data");

                PlayFab.Title.OnResponse += TitleResponseCallback;
                PlayFab.Title.Request();
            }
            else
            {
                Popup.Show("Failed To Connect, Play Offline ?", StartOffline, "Okay");
            }
        }

        void TitleResponseCallback(PlayFabTitleCore data, PlayFabError error)
        {
            PlayFab.Title.OnResponse -= TitleResponseCallback;

            if (error == null)
            {
                Popup.Show("Retrieving Catalog");

                PlayFab.Catalog.OnResponse += CatalogResponseCallback;
                PlayFab.Catalog.Request();
            }
            else
            {
                RaiseError(error);
            }
        }

        void CatalogResponseCallback(PlayFabCatalogCore catalog, PlayFabError error)
        {
            PlayFab.Catalog.OnResponse -= CatalogResponseCallback;

            if (error == null)
            {
                if(PlayFab.IsLoggedIn)
                {
                    Popup.Show("Retrieving Daily Reward");

                    PlayFab.DailyReward.OnResponse += DailyRewardsResponseCallback;
                    PlayFab.DailyReward.Claim();
                }
                else
                {
                    Popup.Show("Retrieving Player Data");

                    PlayFab.Player.OnResponse += PlayerResponseCallback;
                    PlayFab.Player.Retrieve();
                }
            }
            else
            {
                RaiseError(error);
            }
        }

        PlayFabDailyRewardCore.ResultData dailyReward;
        void DailyRewardsResponseCallback(PlayFabDailyRewardCore.ResultData result, PlayFabError error)
        {
            PlayFab.DailyReward.OnResponse -= DailyRewardsResponseCallback;

            if(error == null)
            {
                dailyReward = result;

                Popup.Show("Retrieving Player Data");

                PlayFab.Player.OnResponse += PlayerResponseCallback;
                PlayFab.Player.Retrieve();
            }
            else
            {
                RaiseError(error);
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
                RaiseError(error);
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

            if(dailyReward == null || dailyReward.Items.Length == 0)
            {
                Progress();
            }
            else
            {
                var stacks = ItemStack.From(dailyReward.Items);

                var title = "Daily Reward" + Environment.NewLine;
                title += "Day " + (dailyReward.Progress + 1).ToString() + "/" + PlayFab.DailyReward.Max;

                Core.UI.Rewards.OnFinish += Progress;
                Core.UI.Rewards.Show(title, stacks);
            }
        }

        void RaiseError(PlayFabError error)
        {
            Debug.LogError("Login Error: " + error.GenerateErrorReport());

            Popup.Show(error.ErrorMessage, Retry, "Retry");
        }
    }
}