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
    public class PlayFabTitleCore : PlayFabCore.Module
    {
        [SerializeField]
        protected PlayFabTitleDataCore data;
        public PlayFabTitleDataCore Data { get { return data; } }

        public class Module : PlayFabCore.Module
        {
            public PlayFabTitleCore Title { get { return PlayFab.Title; } }

            new public static string FormatFilePath(string name)
            {
                return PlayFabCore.Module.FormatFilePath("Data/" + name);
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
    public class PlayFabTitleDataCore : PlayFabTitleCore.Module
    {
        public Dictionary<string, string> Value { get; protected set; }

        public virtual void Request()
        {
            if (IsLoggedIn)
            {
                var request = new GetTitleDataRequest
                {

                };

                PlayFabClientAPI.GetTitleData(request, RetrieveCallbac, ErrorCallback);
            }
            else
            {
                LoadResult();
            }
        }

        public const string FileName = "Title Data.json";
        protected virtual void LoadResult()
        {
            var filePath = FormatFilePath(FileName);

            if (Core.Data.Exists(filePath))
            {
                var json = Core.Data.LoadText(filePath);

                var request = JsonConvert.DeserializeObject<GetTitleDataResult>(json);

                RetrieveCallbac(request);
            }
            else
            {
                var error = new PlayFabError()
                {
                    ErrorMessage = "No Local Data Found"
                };

                ErrorCallback(error);
            }
        }
        protected virtual void SaveResult(GetTitleDataResult request)
        {
            var json = JsonConvert.SerializeObject(request, Formatting.Indented);

            Core.Data.Save(FormatFilePath(FileName), json);
        }

        public event Delegates.ResultDelegate<PlayFabTitleDataCore> OnRetrieved;
        void RetrieveCallbac(GetTitleDataResult result)
        {
            Value = result.Data;

            if (OnRetrieved != null) OnRetrieved(this);

            Respond(result, null);

            if (IsLoggedIn)
                SaveResult(result);
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