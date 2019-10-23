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

#if GOOGLE_PLAY
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

namespace Game
{
    public class GoogleLogin : Login.Controller
    {
        public override bool Accessible => true;

        public override bool Available => Application.platform == RuntimePlatform.Android;

#if GOOGLE_PLAY
        public override void Show()
        {
            base.Show();

            Perform();
        }

        public override void Perform()
        {
            base.Perform();

            Popup.Show("Logging In");

            var builder = new PlayGamesClientConfiguration.Builder();

            builder.AddOauthScope("profile");
            builder.RequestServerAuthCode(false);

            var config = builder.Build();

            PlayGamesPlatform.InitializeInstance(config);

            PlayGamesPlatform.DebugLogEnabled = true;

            PlayGamesPlatform.Activate();

            Social.localUser.Authenticate(AuthenticateCallback);
        }

        void AuthenticateCallback(bool success, string error)
        {
            if(success)
            {
                var authCode = PlayGamesPlatform.Instance.GetServerAuthCode();

                PlayFab.Login.Google.Perform(authCode);
            }
            else
            {
                Debug.LogError("Google Login Error: " + error);

                Popup.Show("<Google Error>\n" + error, Login.Reload, "Retry");
            }
        }
#endif
    }
}