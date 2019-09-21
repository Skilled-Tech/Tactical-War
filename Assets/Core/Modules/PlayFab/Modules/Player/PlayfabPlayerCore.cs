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
	public class PlayFabPlayerCore : PlayFabCore.Module
	{
        [SerializeField]
        protected PlayFabPlayerCombinedInfoCore combinedInfo;
        public PlayFabPlayerCombinedInfoCore CombinedInfo { get { return combinedInfo; } }

        [SerializeField]
        protected PlayFabPlayerReadOnlyData readonlyData;
        public PlayFabPlayerReadOnlyData ReadonlyData { get { return readonlyData; } }

        [SerializeField]
        protected PlayFabPlayerInventoryCore inventory;
        public PlayFabPlayerInventoryCore Inventory { get { return inventory; } }

        [Serializable]
		public class Module : PlayFabCore.Module
        {
            public PlayFabPlayerCore Player { get { return PlayFab.Player; } }

            protected override string FormatFilePath(string name)
            {
                return base.FormatFilePath("Player/" + name);
            }
        }

        public virtual void Retrieve()
        {
            readonlyData.OnResponse += ReadOnlyDataResponseCallback;

            readonlyData.Retrieve();
        }

        void ReadOnlyDataResponseCallback(PlayFabPlayerReadOnlyData result, PlayFabError error)
        {
            readonlyData.OnResponse -= ReadOnlyDataResponseCallback;

            if(error == null)
            {
                inventory.OnResponse += InventoryResponseCallback;
                inventory.Request();
            }
            else
            {
                Respond(error);
            }
        }

        void InventoryResponseCallback(PlayFabPlayerInventoryCore result, PlayFabError error)
        {
            inventory.OnResponse -= InventoryResponseCallback;

            Respond(error);
        }

        public event Delegates.ResponseDelegate<PlayFabPlayerCore> OnResponse;
        void Respond(PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);
        }

        public override void Configure()
        {
            base.Configure();

            Register(combinedInfo);
            Register(readonlyData);
        }
    }

    [Serializable]
    public class PlayFabPlayerCombinedInfoCore : PlayFabPlayerCore.Module
    {
        public virtual void Request()
        {
            var request = new GetPlayerCombinedInfoRequest()
            {
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true,
                    GetTitleData = true,
                    GetUserAccountInfo = true,
                    GetUserInventory = true,
                    GetUserReadOnlyData = true,
                    GetUserVirtualCurrency = true,
                    ProfileConstraints = new PlayerProfileViewConstraints()
                    {
                        ShowLastLogin = true,
                    },
                    TitleDataKeys = null,
                    UserReadOnlyDataKeys = null,
                }
            };

            PlayFabClientAPI.GetPlayerCombinedInfo(request, RetrieveCallbac, ErrorCallback);
        }

        public event Delegates.RetrievedDelegate<GetPlayerCombinedInfoResult> OnRetrieved;
        void RetrieveCallbac(GetPlayerCombinedInfoResult result)
        {
            if (OnRetrieved != null) OnRetrieved(result);

            Respond(result, null);
        }
        
        public event Delegates.ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);

            Respond(null, error);
        }

        public event Delegates.ResponseDelegate<GetPlayerCombinedInfoResult> OnResponse;
        void Respond(GetPlayerCombinedInfoResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(result, error);
        }
    }

    [Serializable]
    public class PlayFabPlayerReadOnlyData : PlayFabPlayerCore.Module
    {
        public Dictionary<string, UserDataRecord> Data { get; protected set; }
        public uint Version { get; protected set; }

        public const string FileName = "Readonly Data";

        public virtual void Retrieve()
        {
            if(IsLoggedIn)
            {
                var request = new GetUserDataRequest
                {

                };

                PlayFabClientAPI.GetUserReadOnlyData(request, RetrieveCallback, ErrorCallback);
            }
            else
            {
                Load<GetUserDataResult>(FileName, RetrieveCallback, ErrorCallback);
            }
        }

        public event Delegates.RetrievedDelegate<PlayFabPlayerReadOnlyData> OnRetrieved;
        void RetrieveCallback(GetUserDataResult result)
        {
            this.Data = result.Data;
            this.Version = result.DataVersion;

            if (OnRetrieved != null) OnRetrieved(this);

            Respond(result, null);

            if (IsLoggedIn)
                Save(result, FileName);
        }

        public event Delegates.ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);

            Respond(null, error);
        }

        public event Delegates.ResponseDelegate<PlayFabPlayerReadOnlyData> OnResponse;
        void Respond(GetUserDataResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);
        }
    }
}