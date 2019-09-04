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
        public EmailHandler Email { get; protected set; }
        public class EmailHandler : PlayFabRequestHandler<LoginWithEmailAddressRequest, LoginResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.LoginWithEmailAddress;

            public virtual void Request(string email, string password)
            {
                var request = CreateRequest();

                request.Email = email;
                request.Password = password;

                Send(request);
            }
        }

        public override void Configure()
        {
            base.Configure();

            Email = new EmailHandler();

            Email.OnResponse += ResponseCallback;
        }

        public virtual void Perform()
        {
            Email.Request("Moe4Baker@gmail.com", "Password");
        }

        public event PlayFabRequestsUtility.ResponseDelegate<PlayFabLoginCore> OnResponse;
        void ResponseCallback(LoginResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);
        }
    }
}