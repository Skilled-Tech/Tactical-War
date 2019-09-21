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
using PlayFab.SharedModels;

namespace Game
{
    [Serializable]
    public class PlayFabCore : Core.Module
    {
        [SerializeField]
        protected PlayFabLoginCore login;
        public PlayFabLoginCore Login { get { return login; } }

        public bool IsLoggedIn { get { return PlayFabClientAPI.IsClientLoggedIn(); } }

        public bool Activated { get; set; }

        [SerializeField]
        public bool startOffline = false;

        [SerializeField]
        protected PlayFabDailyRewardCore dailyRewards;
        public PlayFabDailyRewardCore DailyRewards { get { return dailyRewards; } }

        [SerializeField]
        protected PlayFabPlayerCore player;
        public PlayFabPlayerCore Player { get { return player; } }

        [SerializeField]
        protected PlayFabTitleCore title;
        public PlayFabTitleCore Title { get { return title; } }

        [SerializeField]
        protected PlayFabCatalogCore catalog;
        public PlayFabCatalogCore Catalog { get { return catalog; } }

        [SerializeField]
        protected PlayFabPurchaseCore purchase;
        public PlayFabPurchaseCore Purchase { get { return purchase; } }

        [SerializeField]
        protected PlayFabUpgradeCore upgrade;
        public PlayFabUpgradeCore Upgrade { get { return upgrade; } }

        [Serializable]
        public class Module : Core.Module
        {
            public PlayFabCore PlayFab { get { return Core.PlayFab; } }

            public bool IsLoggedIn { get { return PlayFab.IsLoggedIn; } }

            protected virtual string FormatFilePath(string name)
            {
                return "PlayFab/" + name + ".json";
            }
            protected virtual void Load<TResult>(string fileName, Action<TResult> resultCallback, Action<PlayFabError> errorCallback)
                where TResult : PlayFabResultCommon
            {
                fileName = FormatFilePath(fileName);

                if (Core.Data.Exists(fileName))
                {
                    var json = Core.Data.LoadText(fileName);

                    var result = JsonConvert.DeserializeObject<TResult>(json);

                    resultCallback(result);
                }
                else
                {
                    var error = new PlayFabError()
                    {
                        ErrorMessage = "No Local " + fileName + " Data Found"
                    };

                    errorCallback(error);
                }
            }
            protected virtual void Save<TResult>(TResult request, string path)
                where TResult : PlayFabResultCommon
            {
                path = FormatFilePath(path);

                var json = JsonConvert.SerializeObject(request, Formatting.Indented);

                Core.Data.Save(path, json);
            }

            public static class Delegates
            {
                public delegate void RetrievedDelegate<TModule, TResult>(TModule module, TResult result);
                public delegate void RetrievedDelegate<TResult>(TResult module);

                public delegate void ResultDelegate<TModule, TResult>(TModule module, TResult result);
                public delegate void ResultDelegate<TResult>(TResult result);

                public delegate void ErrorDelegate(PlayFabError error);

                public delegate void ResponseDelegate<TModule, TResult> (TModule module, TResult result, PlayFabError error);
                public delegate void ResponseDelegate<TResult> (TResult result, PlayFabError error);
            }
        }

        public override void Configure()
        {
            base.Configure();

            Activated = false;

            Register(login);
            Register(dailyRewards);
            Register(player);
            Register(title);
            Register(catalog);
            Register(purchase);
            Register(upgrade);
        }

        public virtual void EnsureActivation()
        {
            if (Activated)
            {

            }
            else
            {
                Core.Scenes.Load(Core.Scenes.Login.Name);
            }
        }
    }

    [Serializable]
    public class PlayFabDailyRewardCore : PlayFabCore.Module
    {
        public virtual void Perform()
        {
            var request = new ExecuteCloudScriptRequest()
            {
                FunctionName = "ProcessDailyReward",

                FunctionParameter = null,

                GeneratePlayStreamEvent = true,
            };

            PlayFabClientAPI.ExecuteCloudScript(request, ResultCallback, ErrorCallback);
        }

        public event Delegates.ResultDelegate<ExecuteCloudScriptResult> OnResult;
        void ResultCallback(ExecuteCloudScriptResult result)
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

        public event Delegates.ResponseDelegate<ExecuteCloudScriptResult> OnResponse;
        void Respond(ExecuteCloudScriptResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(result, error);
        }
    }
}