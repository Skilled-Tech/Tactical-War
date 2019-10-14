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
    public class PlayFabLoginCore : PlayFabCore.Property
    {
        [SerializeField]
        protected GoogleProperty google;
        public GoogleProperty Google { get { return google; } }
        [Serializable]
        public class GoogleProperty : Property
        {
            public virtual void Perform(string authCode)
            {
                var request = new LoginWithGoogleAccountRequest
                {
                    ServerAuthCode = authCode,
                    CreateAccount = true
                };

                PlayFabClientAPI.LoginWithGoogleAccount(request, ResultCallback, ErrorCallback);
            }
        }
        
        [SerializeField]
        protected EmailProperty email;
        public EmailProperty Email { get { return email; } }
        [Serializable]
        public class EmailProperty : Property
        {
            public virtual void Perform(string email, string password)
            {
                var request = new LoginWithEmailAddressRequest
                {
                    Email = email,
                    Password = password,
                };

                PlayFabClientAPI.LoginWithEmailAddress(request, ResultCallback, ErrorCallback);
            }
        }

        [Serializable]
        public class Property : PlayFabCore.Property
        {
            public event Delegates.RetrievedDelegate<LoginResult> OnResult;
            protected virtual void ResultCallback(LoginResult result)
            {
                if (OnResult != null) OnResult(result);

                Respond(result, null);
            }

            public event Delegates.ErrorDelegate OnError;
            protected virtual void ErrorCallback(PlayFabError error)
            {
                if (OnError != null) OnError(error);

                Respond(null, error);
            }

            public event Delegates.ResponseDelegate<LoginResult> OnResponse;
            protected virtual void Respond(LoginResult result, PlayFabError error)
            {
                if (OnResponse != null) OnResponse(result, error);
            }
        }

        public override void Configure()
        {
            base.Configure();

            Register(google);
            Register(email);
        }

        public virtual void Register(Property property)
        {
            base.Register(property);

            property.OnResult += ResultCallback;
            property.OnError += ErrorCallback;
        }

        public event Delegates.RetrievedDelegate<LoginResult> OnResult;
        void ResultCallback(LoginResult result)
        {
            if (OnResult != null) OnResult(result);

            Respond(result, null);
        }

        public event Delegates.ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);

            Respond(null, error);
        }

        public event Delegates.ResponseDelegate<LoginResult> OnResponse;
        void Respond(LoginResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(result, error);
        }
    }
}