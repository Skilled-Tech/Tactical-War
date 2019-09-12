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
    [Serializable]
    public class PlayFabLoginCore : PlayFabCore.Module
    {
        public virtual void Perform()
        {
            if (Application.isEditor)
            {
                var request = new LoginWithEmailAddressRequest
                {
                    Email = "Moe4Baker@gmail.com",
                    Password = "Password",
                };

                PlayFabClientAPI.LoginWithEmailAddress(request, ResultCallback, ErrorCallback);
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                var request = new LoginWithAndroidDeviceIDRequest
                {
                    AndroidDevice = SystemInfo.deviceModel,
                    AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
                    OS = SystemInfo.operatingSystem,
                    CreateAccount = true
                };

                PlayFabClientAPI.LoginWithAndroidDeviceID(request, ResultCallback, ErrorCallback);
            }
            else
            {
                var request = new LoginWithEmailAddressRequest
                {
                    Email = "Moe4Baker@gmail.com",
                    Password = "Password",
                };

                PlayFabClientAPI.LoginWithEmailAddress(request, ResultCallback, ErrorCallback);
            }
        }

        public delegate void ResultDelegate(PlayFabLoginCore login, LoginResult result);
        public event ResultDelegate OnResult;
        void ResultCallback(LoginResult result)
        {
            if (OnResult != null) OnResult(this, result);

            Respond(result, null);
        }

        public delegate void ErrorDelegate(PlayFabError error);
        public event ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);

            Respond(null, error);
        }

        public delegate void ResponseCallback(PlayFabLoginCore login, LoginResult result, PlayFabError error);
        public event ResponseCallback OnResponse;
        void Respond(LoginResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, result, error);
        }
    }
}