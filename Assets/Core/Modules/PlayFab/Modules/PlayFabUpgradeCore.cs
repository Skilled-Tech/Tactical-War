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

            PlayFabClientAPI.ExecuteCloudScript(request, ResultCallback, ErrorCallback);
        }
        public virtual void Perform(ItemInstance itemInstance, string type)
        {
            Perform(itemInstance.ItemInstanceId, type);
        }
        public virtual void Perform(ItemInstance itemInstance, ItemUpgradeType type)
        {
            Perform(itemInstance, type.ID);
        }
        public virtual void Perform(PlayerInventoryCore.ItemData item, ItemUpgradeType type)
        {
            Perform(item.Instance, type.ID);
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