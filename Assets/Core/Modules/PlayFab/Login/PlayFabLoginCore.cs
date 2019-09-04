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
    [CreateAssetMenu(menuName = MenuPath + "Login")]
	public class PlayFabLoginCore : PlayFabCore.Module
	{
        public EmaiLoginHandler EmailLogin { get; protected set; }
        public class EmaiLoginHandler : PlayFabRequestHandler<LoginWithEmailAddressRequest, LoginResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.LoginWithEmailAddress;

            public virtual void Send(string email, string password)
            {
                var request = CreateRequest();

                request.Email = email;
                request.Password = password;

                Send(request);
            }
        }

        public AndroidLoginHandler AndroidIDLogin;
        public class AndroidLoginHandler : PlayFabRequestHandler<LoginWithAndroidDeviceIDRequest, LoginResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.LoginWithAndroidDeviceID;

            public virtual void Send()
            {
                var request = CreateRequest();

                request.CreateAccount = true;

                request.AndroidDeviceId = SystemInfo.deviceUniqueIdentifier;
                request.AndroidDevice = SystemInfo.deviceModel;
                request.OS = SystemInfo.operatingSystem;

                Send(request);
            }
        }

        public override void Configure()
        {
            base.Configure();

            EmailLogin = new EmaiLoginHandler();
            EmailLogin.OnResponse += ResponseCallback;

            AndroidIDLogin = new AndroidLoginHandler();
            AndroidIDLogin.OnResponse += ResponseCallback;
        }

        public virtual void Perform()
        {
            if(Application.isEditor)
            {
                EmailLogin.Send("Moe4Baker@gmail.com", "Password");
            }
            else if(Application.platform == RuntimePlatform.Android)
            {
                AndroidIDLogin.Send();
            }
            else
            {

            }
        }

        public event PlayFabRequestsUtility.ResponseDelegate<PlayFabLoginCore> OnResponse;
        void ResponseCallback(LoginResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);
        }
    }
}