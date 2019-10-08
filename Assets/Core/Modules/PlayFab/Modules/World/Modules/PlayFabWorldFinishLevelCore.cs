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

using PlayFabSDK = PlayFab;

using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;

using Newtonsoft.Json;

namespace Game
{
    [Serializable]
	public class PlayFabWorldFinishLevelCore : PlayFabWorldCore.Module
    {
        public class ParametersData
        {
            public string region;
            public int level;
            public int difficulty;

            public ParametersData(string region, int level, int difficulty)
            {
                this.region = region;
                this.level = level;
                this.difficulty = difficulty;
            }
        }
        
        public virtual void Request(string region, int level, int difficulty)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "FinishLevel",
                FunctionParameter = new ParametersData(region, level, difficulty),
                GeneratePlayStreamEvent = true,
            };

            PlayFabClientAPI.ExecuteCloudScript(request, ResultCallback, ErrorCallback);
        }
        public virtual void Request(LevelCore level, RegionDifficulty difficulty)
        {
            Request(level.Region.name, level.Index, level.Region.Difficulty.ID);
        }
        public virtual void Request(LevelCore level)
        {
            Request(level, level.Region.Difficulty);
        }

        public event Delegates.ResultDelegate<ResultData> OnResult;
        void ResultCallback(ExecuteCloudScriptResult result)
        {
            var json = result.FunctionResult.ToString();

            var instacne = JsonConvert.DeserializeObject<ResultData>(json);

            if (OnResult != null) OnResult(instacne);

            Respond(instacne, null);
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
            ItemTemplate[] rewards;
            public ItemTemplate[] Rewards => rewards;
        }
#pragma warning restore CS0649
    }
}