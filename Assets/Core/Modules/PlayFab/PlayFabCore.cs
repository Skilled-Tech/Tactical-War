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
    public class PlayFabCore : Core.Module
	{
        [SerializeField]
        protected PlayFabLoginCore login;
        public PlayFabLoginCore Login { get { return login; } }

        [SerializeField]
        protected PlayFabTitleCore title;
        public PlayFabTitleCore Title { get { return title; } }

        [SerializeField]
        protected PlayFabCatalogCore catalog;
        public PlayFabCatalogCore Catalog { get { return catalog; } }

        [SerializeField]
        protected PlayFabPurchaseCore purchase;
        public PlayFabPurchaseCore Purchase { get { return purchase; } }

        [SerializeField]
        protected PlayFabUpgradeCore upgrade;
        public PlayFabUpgradeCore Upgrade { get { return upgrade; } }

        [Serializable]
        public class Module : Core.Module
        {
            public PlayFabCore PlayFab { get { return Core.PlayFab; } }
        }
        
        public virtual void ForAllElements(Action<Module> action)
        {
            action(login);
            action(title);
            action(catalog);
            action(purchase);
            action(upgrade);
        }
        
        public override void Configure()
        {
            base.Configure();

            ForAllElements(x => x.Configure());
        }

        public override void Init()
        {
            base.Init();

            ForAllElements(x => x.Init());
        }

        public static class Utility
        {
            public delegate void ResaultDelegate<TResult>(TResult result);

            public delegate void ResponseDelegate<TResult>(TResult result, PlayFabError error);
        }

        public abstract class RequestHandler<TRequest, TResult>
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

            public event PlayFabCore.Utility.ResaultDelegate<TResult> OnResult;
            public virtual void ResultCallback(TResult result)
            {
                Latest = result;

                if (OnResult != null) OnResult(result);

                Respond(result, null);
            }

            public event PlayFabCore.Utility.ResponseDelegate<TResult> OnResponse;
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
}