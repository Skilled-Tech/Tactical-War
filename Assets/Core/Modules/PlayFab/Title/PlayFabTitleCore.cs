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

using PlayFab.ClientModels;
using PlayFab;

namespace Game
{
    [CreateAssetMenu(menuName = MenuPath + "Title")]
	public class PlayFabTitleCore : PlayFabCore.Module
	{
        [SerializeField]
        protected DataCore data;
        new public DataCore Data { get { return data; } }
        [Serializable]
        public class DataCore
        {
            public GetRequestHandler GetRequest { get; protected set; }
            public class GetRequestHandler : PlayFabRequestHandler<GetTitleDataRequest, GetTitleDataResult>
            {
                public override AskDelegate Ask => PlayFabClientAPI.GetTitleData;

                public virtual void Send()
                {
                    var request = CreateRequest();

                    Send(request);
                }
            }

            public Dictionary<string, string> Value { get { return GetRequest.Latest.Data; } }

            public virtual void Configure()
            {
                GetRequest = new GetRequestHandler();

                GetRequest.OnResponse += ResponseCallback;
            }

            public virtual void Request()
            {
                GetRequest.Send();
            }

            public event PlayFabRequestsUtility.ResponseDelegate<DataCore> OnResponse;
            void ResponseCallback(GetTitleDataResult result, PlayFabError error)
            {
                if (OnResponse != null) OnResponse(this, error);
            }
        }

        public override void Configure()
        {
            base.Configure();

            data.Configure();
        }

        public virtual void Request()
        {
            Data.OnResponse += OnDataResponse;

            Data.Request();
        }

        void OnDataResponse(DataCore result, PlayFabError error)
        {
            ResponseCompleted(error);
        }

        public event PlayFabRequestsUtility.ResponseDelegate<PlayFabTitleCore> OnResponse;
        public virtual void ResponseCompleted(PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);
        }
    }

    public static class PlayFabRequestsUtility
    {
        public delegate void ResaultDelegate<TResult>(TResult result);

        public delegate void ResponseDelegate<TResult>(TResult result, PlayFabError error);
    }

    public abstract class PlayFabRequestHandler<TRequest, TResult>
        where TRequest : class, new()
        where TResult : class
    {
        public TResult Latest { get; protected set; }
        public virtual void Clear()
        {
            Latest = null;
        }

        public delegate void AskDelegate(TRequest request, Action<TResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null);
        public abstract AskDelegate Ask { get; }

        public virtual void Send(TRequest request)
        {
            Clear();

            Ask(request, ResultCallback, ErrorCallback);
        }

        public virtual TRequest CreateRequest()
        {
            return new TRequest();
        }

        public event PlayFabRequestsUtility.ResaultDelegate<TResult> OnResult;
        public virtual void ResultCallback(TResult result)
        {
            Latest = result;

            if (OnResult != null) OnResult(result);

            Respond(result, null);
        }

        public event PlayFabRequestsUtility.ResponseDelegate<TResult> OnResponse;
        public virtual void Respond(TResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(result, error);
        }

        public delegate void ErrorDelegate(PlayFabError error);
        public event ErrorDelegate OnError;
        public virtual void ErrorCallback(PlayFabError error)
        {
            Debug.LogError("Error On Request, Report: " + error.GenerateErrorReport());

            if (OnError != null) OnError(error);

            Respond(null, error);
        }
    }
}