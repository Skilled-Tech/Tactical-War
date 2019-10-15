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

using Google;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace Game
{
	public class GoogleLogin : Login.Module
	{
        public override void Show()
        {
            base.Show();

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

                Popup.Show("<Google Error>\n" + error, Login.Retry, "Retry");
            }
        }
    }
}