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
    public class PlayFabTitleCore : PlayFabCore.Module
	{
        [SerializeField]
        protected PlayFabTitleDataCore data;
        public PlayFabTitleDataCore Data { get { return data; } }

        public override void Configure()
        {
            base.Configure();

            data.Configure();
        }

        public virtual void Request()
        {
            Data.OnResponse += OnDataResponse;

            Data.Request();
        }

        void OnDataResponse(PlayFabTitleDataCore title, PlayFabError error)
        {
            Data.OnResponse -= OnDataResponse;

            ResponseCompleted(error);
        }

        public delegate void ResponseCallback(PlayFabTitleCore title, PlayFabError error);
        public event ResponseCallback OnResponse;
        public virtual void ResponseCompleted(PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);
        }
    }

    [Serializable]
    public class PlayFabTitleDataCore : PlayFabCore.Module
    {
        public Dictionary<string, string> Value { get; protected set; }

        public virtual void Request()
        {
            var request = new GetTitleDataRequest
            {

            };

            PlayFabClientAPI.GetTitleData(request, ResultCallback, ErrorCallback);
        }

        public delegate void ResultDelegate(PlayFabTitleDataCore data);
        public event ResultDelegate OnRetrieved;
        void ResultCallback(GetTitleDataResult result)
        {
            Value = result.Data;

            if (OnRetrieved != null) OnRetrieved(this);

            Respond(result, null);
        }

        public delegate void ErrorDelegate(PlayFabError error);
        public event ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);

            Respond(null, error);
        }

        public delegate void ResponseCallback(PlayFabTitleDataCore data, PlayFabError error);
        public event ResponseCallback OnResponse;
        void Respond(GetTitleDataResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);
        }
    }
}