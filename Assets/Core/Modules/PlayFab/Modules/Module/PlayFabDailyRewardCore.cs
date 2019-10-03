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
using PlayFab.Json;
using PlayFab.ClientModels;
using PlayFab.SharedModels;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Game
{
    [Serializable]
    public class PlayFabLoginReward : PlayFabRewardCore.Module
    {
        public const string ID = "daily-rewards";

        protected int max;
        public int Max { get { return max; } }

        public override void Configure()
        {
            base.Configure();

            PlayFab.Title.Data.OnRetrieved += TitleDataRetrieveCallback;
        }

        void TitleDataRetrieveCallback(PlayFabTitleDataCore result)
        {
            var jArray = JArray.Parse(result.Value[ID]);

            max = jArray.Count;
        }

        #region Request
        public virtual void Claim()
        {
            var request = new ExecuteCloudScriptRequest()
            {
                FunctionName = "ClaimDailyReward",

                FunctionParameter = null,

                GeneratePlayStreamEvent = true,
            };

            PlayFabClientAPI.ExecuteCloudScript(request, ResultCallback, ErrorCallback);
        }

        public event Delegates.ResultDelegate<ResultData> OnResult;
        void ResultCallback(ExecuteCloudScriptResult result)
        {
            ResultData data = null;

            if (result.FunctionResult == null)
                data = null;
            else
                data = new ResultData(result.FunctionResult as JsonObject);

            if (OnResult != null) OnResult(data);

            Respond(data, null);
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

        [Newtonsoft.Json.JsonObject]
        [Serializable]
        public class ResultData
        {
            [Newtonsoft.Json.JsonProperty]
            protected int progress;
            public int Progress { get { return progress; } }

            [Newtonsoft.Json.JsonProperty(ItemConverterType = typeof(ItemTemplate.Converter))]
            protected ItemTemplate[] items;
            public ItemTemplate[] Items { get { return items; } }

            public ResultData(string json)
            {
                JsonConvert.PopulateObject(json, this);
            }
            public ResultData(JsonObject jObject) : this(jObject.ToString())
            {

            }
        }
        #endregion
    }
}