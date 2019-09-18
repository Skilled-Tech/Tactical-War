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
	public class PlayfabPlayerCore : PlayFabCore.Module
	{
        [SerializeField]
        protected PlayFabPlayerCombinedInfoCore combinedInfo;
        public PlayFabPlayerCombinedInfoCore CombinedInfo { get { return combinedInfo; } }

        [SerializeField]
        protected PlayFabPlayerReadOnlyData readonlyData;
        public PlayFabPlayerReadOnlyData ReadonlyData { get { return readonlyData; } }

        [Serializable]
		public class Module : PlayFabCore.Module
        {
            public PlayfabPlayerCore Player { get { return PlayFab.Player; } }
        }

        public override void Configure()
        {
            base.Configure();

            Register(combinedInfo);
            Register(readonlyData);
        }
    }

    [Serializable]
    public class PlayFabPlayerCombinedInfoCore : PlayfabPlayerCore.Module
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

        public event Delegates.ResponseCallback<GetPlayerCombinedInfoResult> OnResponse;
        void Respond(GetPlayerCombinedInfoResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(result, error);
        }
    }

    [Serializable]
    public class PlayFabPlayerReadOnlyData : PlayfabPlayerCore.Module
    {
        public Dictionary<string, UserDataRecord> Data { get; protected set; }
        public uint Version { get; protected set; }

        public virtual void Request()
        {
            var request = new GetUserDataRequest
            {
                
            };

            PlayFabClientAPI.GetUserReadOnlyData(request, RetrieveCallback, ErrorCallback);
        }

        public event Delegates.RetrievedDelegate<PlayFabPlayerReadOnlyData> OnRetrieved;
        void RetrieveCallback(GetUserDataResult result)
        {
            this.Data = result.Data;
            this.Version = result.DataVersion;

            if (OnRetrieved != null) OnRetrieved(this);

            Respond(result, null);
        }

        public event Delegates.ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);

            Respond(null, error);
        }

        public event Delegates.ResponseCallback<PlayFabPlayerReadOnlyData> OnResponse;
        void Respond(GetUserDataResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);
        }
    }
}