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

        public GuestLogin GuestLogin { get; protected set; }

        public abstract class Module : UIElementModule<Login>
        {
            public Login Login { get { return Reference; } }

            #region Accessors
            public static Core Core { get { return Core.Instance; } }

            public static PopupUI Popup { get { return Core.UI.Popup; } }

            public static ScenesCore Scenes { get { return Core.Scenes; } }

            public static PlayerCore Player { get { return Core.Player; } }

            public static PlayFabCore PlayFab { get { return Core.PlayFab; } }
            #endregion
        }

        public abstract class Controller : Module
        {
            [SerializeField]
            protected Button entry;
            public Button Entry { get { return entry; } }

            public abstract bool IsValid { get; }

            protected virtual void UpdateState()
            {
                entry.interactable = IsValid;
            }

            public override void Init()
            {
                base.Init();

                UpdateState();
            }
        }

        #region Accessors
        public static Core Core { get { return Core.Instance; } }

        public static PopupUI Popup { get { return Core.UI.Popup; } }

        public static ScenesCore Scenes { get { return Core.Scenes; } }

        public static PlayerCore Player { get { return Core.Player; } }

        public static PlayFabCore PlayFab { get { return Core.PlayFab; } }
        #endregion

        private void Awake()
        {
            Google = Dependancy.Get<GoogleLogin>(gameObject);
            Email = Dependancy.Get<EmailLogin>(gameObject);
            GuestLogin = Dependancy.Get<GuestLogin>(gameObject);

            Modules.Configure(this);
        }

        private void Start()
        {
            PlayFab.Login.OnResponse += LoginResponseCallback;

            Modules.Init(this);
        }

        public virtual void Retry()
        {
            Scenes.Load(Scenes.Login);

            Popup.Hide();
        }

        public virtual void Offline()
        {
            PlayFab.Title.OnResponse += TitleResponseCallback;
            PlayFab.Title.Request();
        }

        void LoginResponseCallback(LoginResult result, PlayFabError error)
        {
            PlayFab.Login.OnResponse -= LoginResponseCallback;

            Core.Prefs.NeedOnlineLogin.Value = false;

            if (error == null)
            {
                Popup.Show("Retrieving Title Data");

                PlayFab.Title.OnResponse += TitleResponseCallback;
                PlayFab.Title.Request();
            }
            else
            {
                RaiseError(error);
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