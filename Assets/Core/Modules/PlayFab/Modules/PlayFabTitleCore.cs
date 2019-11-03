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

using Newtonsoft.Json;

namespace Game
{
    [Serializable]
    public class PlayFabTitleCore : PlayFabCore.Property
    {
        [SerializeField]
        protected PlayFabTitleDataCore data;
        public PlayFabTitleDataCore Data { get { return data; } }

        public class Module : PlayFabCore.Property
        {
            public PlayFabTitleCore Title { get { return PlayFab.Title; } }

            protected override string FormatFilePath(string name)
            {
                return base.FormatFilePath("Title/" + name);
            }
        }

        public override void Configure()
        {
            base.Configure();

            Register(data);
        }

        public virtual void Request()
        {
            Data.OnResponse += OnDataResponse;

            Data.Request();
        }

        void OnDataResponse(PlayFabTitleDataCore title, PlayFabError error)
        {
            Data.OnResponse -= OnDataResponse;

            Respond(error);
        }

        public event Delegates.ResponseDelegate<PlayFabTitleCore> OnResponse;
        void Respond(PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);

            if(error == null)
            {
                RetrieveCallback();
            }
            else
            {
                ErrorCallback(error);
            }
        }

        public event Delegates.ResultDelegate<PlayFabTitleCore> OnRetrieved;
        void RetrieveCallback()
        {
            if (OnRetrieved != null) OnRetrieved(this);
        }

        public event Delegates.ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);
        }
    }

    [Serializable]
    public class PlayFabTitleDataCore : PlayFabTitleCore.Module
    {
        public Dictionary<string, string> Value { get; protected set; }

        public const string FileName = "Data";

        public virtual void Request()
        {
            if (IsOnline)
            {
                var request = new GetTitleDataRequest
                {

                };

                PlayFabClientAPI.GetTitleData(request, RetrieveCallback, ErrorCallback);
            }
            else
            {
                Load<GetTitleDataResult>(FileName, RetrieveCallback, ErrorCallback);
            }
        }

        public event Delegates.ResultDelegate<PlayFabTitleDataCore> OnRetrieved;
        void RetrieveCallback(GetTitleDataResult result)
        {
            Value = result.Data;

            if (OnRetrieved != null) OnRetrieved(this);

            Respond(result, null);

            if (IsOnline)
                Save(result, FileName);
        }

        public event Delegates.ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);

            Respond(null, error);
        }

        public event Delegates.ResponseDelegate<PlayFabTitleDataCore> OnResponse;
        void Respond(GetTitleDataResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);
        }
    }
}