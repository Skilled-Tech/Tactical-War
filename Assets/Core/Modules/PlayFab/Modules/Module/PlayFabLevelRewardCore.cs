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

namespace Game
{
    [Serializable]
    public class PlayFabLevelRewardCore : PlayFabRewardCore.Module
    {
        public class ParametersData
        {
            public string region;
            public int level;

            public ParametersData(string region, int level)
            {
                this.region = region;
                this.level = level;
            }
        }

        public virtual void Retrieve(string region, int level)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "FinishLevel",
                FunctionParameter = new ParametersData(region, level),
                GeneratePlayStreamEvent = true,
            };

            PlayFabClientAPI.ExecuteCloudScript(request, ResultCallback, ErrorCallback);
        }
        public virtual void Retrieve(LevelCore level)
        {
            Retrieve(level.Region.name, level.Index);
        }

        public event Delegates.ResultDelegate<IList<ItemStack>> OnResult;
        void ResultCallback(ExecuteCloudScriptResult result)
        {
            IList<ItemStack> data = null;

            if (result.FunctionResult == null)
            {
                data = null;
            }
            else
            {
                var array = result.FunctionResult as JsonArray;

                if (array.Count == 0)
                {
                    data = null;
                }
                else
                {
                    var IDs = array.ConvertAll(element => (string)element);

                    data = ItemStack.From(IDs);
                }
            }

            if (OnResult != null) OnResult(data);

            Respond(data, null);
        }

        public event Delegates.ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);

            Respond(null, error);
        }

        public event Delegates.ResponseDelegate<IList<ItemStack>> OnResponse;
        void Respond(IList<ItemStack> result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(result, error);
        }
    }
}