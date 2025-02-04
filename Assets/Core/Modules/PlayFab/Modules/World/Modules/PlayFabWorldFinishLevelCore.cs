﻿using System;
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
            public float time;

            public ParametersData(string region, int level, int difficulty, float time)
            {
                this.region = region;
                this.level = level;
                this.difficulty = difficulty;
                this.time = time;
            }
        }
        
        public virtual void Request(string region, int level, int difficulty, float time)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "FinishLevel",
                FunctionParameter = new ParametersData(region, level, difficulty, time),
                GeneratePlayStreamEvent = true,
            };

            PlayFabClientAPI.ExecuteCloudScript(request, ResultCallback, ErrorCallback);
        }
        public virtual void Request(LevelCore level, RegionDifficulty difficulty, float time)
        {
            Request(level.Region.name, level.Index, difficulty.ID, time);
        }
        public virtual void Request(LevelCore level, float time)
        {
            Request(level, level.Region.Progress.Difficulty, time);
        }

        public event Delegates.ResultDelegate<ResultData> OnResult;
        void ResultCallback(ExecuteCloudScriptResult result)
        {
            ResultData instance;

            if(result.FunctionResult == null)
            {
                instance = null;
            }
            else
            {
                var json = result.FunctionResult.ToString();

                instance = JsonConvert.DeserializeObject<ResultData>(json);
            }

            if (OnResult != null) OnResult(instance);
            Respond(instance, null);
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

            [JsonProperty]
            protected int stars;
            public int Stars => stars;

            public ResultData()
            {
                rewards = new ItemTemplate[] { };
            }
        }
#pragma warning restore CS0649
    }
}