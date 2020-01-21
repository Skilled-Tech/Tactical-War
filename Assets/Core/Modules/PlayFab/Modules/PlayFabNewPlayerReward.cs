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
using Newtonsoft.Json.Linq;

namespace Game
{
    [Serializable]
	public class PlayFabNewPlayerReward : PlayFabCore.Property
	{
        [SerializeField]
        protected ItemTemplate token;
        public ItemTemplate Toekn { get { return token; } }

        public virtual bool CanRequest
        {
            get
            {
                if (PlayFab.isOffline)
                    return false;

                if (PlayFab.Player.Inventory.Contains(token.CatalogItem.ItemId))
                    return false;

                return true;
            }
        }

        class ParametersData
        {

            public ParametersData()
            {
                
            }
        }

        public virtual void Request()
        {
            if(CanRequest == false)
            {
                Debug.LogError("Can't Request New Player Reward At The Moment, Ignoring");
                return;
            }

            var request = new ExecuteCloudScriptRequest()
            {
                FunctionName = "WelcomeNewPlayer",

                FunctionParameter = new ParametersData(),

                GeneratePlayStreamEvent = true,
            };

            PlayFabClientAPI.ExecuteCloudScript(request, ResultCallback, ErrorCallback);
        }

        public event Delegates.ResultDelegate<ResultData> OnResult;
        void ResultCallback(ExecuteCloudScriptResult result)
        {
            if(result.FunctionResult == null)
            {
                var error = new PlayFabError()
                {
                    ErrorMessage = "Error with New Player Reward"
                };

                ErrorCallback(error);
            }
            else
            {
                var json = result.FunctionResult.ToString();

                var instance = JsonConvert.DeserializeObject<ResultData>(json);

                if (OnResult != null) OnResult(instance);

                Respond(instance, null);
            }
        }

        public event Delegates.ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);

            Respond(null, error);
        }

        public event Delegates.ResponseDelegate<ResultData> OnResponse;
        void Respond(ResultData result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(result, error);
        }

#pragma warning disable CS0649
        [JsonObject]
        [Serializable]
        public class ResultData
        {
            [JsonProperty(ItemConverterType = typeof(ItemTemplate.Converter))]
            protected ItemTemplate[] items;
            public ItemTemplate[] Items { get { return items; } }

            public ResultData()
            {

            }
        }
#pragma warning restore CS0649
    }
}