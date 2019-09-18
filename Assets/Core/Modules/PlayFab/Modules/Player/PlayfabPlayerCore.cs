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

            PlayFabClientAPI.GetPlayerCombinedInfo(request, ResultCallback, ErrorCallback);
        }

        public event Delegates.ResultDelegate<GetPlayerCombinedInfoResult> OnResult;
        void ResultCallback(GetPlayerCombinedInfoResult result)
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

        public override void Configure()
        {
            base.Configure();

            Player.CombinedInfo.OnResult += CombinedDataRetrievedCallback;
        }

        private void CombinedDataRetrievedCallback(GetPlayerCombinedInfoResult result)
        {
            
        }

        public virtual void Request()
        {
            var request = new GetUserDataRequest
            {
                
            };

            PlayFabClientAPI.GetUserReadOnlyData(request, ResultCallback, ErrorCallback);
        }

        public event Delegates.ResultDelegate<PlayFabPlayerReadOnlyData> OnResult;
        void ResultCallback(GetUserDataResult result)
        {
            Load(result.Data, result.DataVersion);

            if (OnResult != null) OnResult(this);

            Respond(result, null);
        }

        public event Delegates.LoadDelegate<PlayFabPlayerReadOnlyData> OnLoad;
        void Load(Dictionary<string, UserDataRecord> data, uint version)
        {
            this.Data = data;
            this.Version = version;

            if (OnLoad != null) OnLoad(this);
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