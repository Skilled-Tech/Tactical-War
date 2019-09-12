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
    public class PlayFabUpgradeCore : PlayFabCore.Module
    {
        struct ParametersData
        {
            public string itemInstanceId;
            public string upgradeType;

            public ParametersData(string itemInstanceId, string upgradeType)
            {
                this.itemInstanceId = itemInstanceId;
                this.upgradeType = upgradeType;
            }
        }

        public virtual void Perform(string itemInstanceID, string type)
        {
            var request = new ExecuteCloudScriptRequest()
            {
                FunctionName = "UpgradeItem",

                FunctionParameter = new ParametersData(itemInstanceID, type),

                GeneratePlayStreamEvent = true,
            };

            Debug.LogError(itemInstanceID);
            Debug.LogError(type);

            PlayFabClientAPI.ExecuteCloudScript(request, ResultCallback, ErrorCallback);
        }
        public virtual void Perform(ItemInstance itemInstance, string type)
        {
            Perform(itemInstance.ItemInstanceId, type);
        }

        public delegate void ResultDelegate(PlayFabUpgradeCore upgrade, ExecuteCloudScriptResult result);
        public event ResultDelegate OnResult;
        void ResultCallback(ExecuteCloudScriptResult result)
        {
            if (OnResult != null) OnResult(this, result);

            Respond(result, null);
        }

        public delegate void ErrorDelegate(PlayFabError error);
        public event ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);

            Respond(null, error);
        }

        public delegate void ResponseCallback(PlayFabUpgradeCore upgrade, ExecuteCloudScriptResult result, PlayFabError error);
        public event ResponseCallback OnResponse;
        void Respond(ExecuteCloudScriptResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, result, error);
        }
    }
}