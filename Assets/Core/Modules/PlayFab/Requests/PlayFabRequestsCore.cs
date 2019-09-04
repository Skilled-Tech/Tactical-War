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
    [CreateAssetMenu(menuName = MenuPath + "Requests")]
	public class PlayFabRequestsCore : PlayFabCore.Module
	{
        

        public GetCatalogHandler GetCatalog { get; protected set; }
        public class GetCatalogHandler : PlayFabRequestHandler<GetCatalogItemsRequest, GetCatalogItemsResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.GetCatalogItems;

            public virtual void Request(string version)
            {
                var request = CreateRequest();

                request.CatalogVersion = version;

                Send(request);
            }
        }

        public GetUserInventoryHandler GetUserInventory { get; protected set; }
        public class GetUserInventoryHandler : PlayFabRequestHandler<GetUserInventoryRequest, GetUserInventoryResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.GetUserInventory;

            public virtual void Request()
            {
                var request = CreateRequest();

                Send(request);
            }
        }

        public ListUserCharactersHandler ListUserCharacters { get; protected set; }
        public class ListUserCharactersHandler : PlayFabRequestHandler<ListUsersCharactersRequest, ListUsersCharactersResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.GetAllUsersCharacters;

            public virtual void Request()
            {
                var request = CreateRequest();

                Send(request);
            }
        }

        public PurchaseItemHandler PurchaseItem { get; protected set; }
        public class PurchaseItemHandler : PlayFabRequestHandler<PurchaseItemRequest, PurchaseItemResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.PurchaseItem;

            public virtual void Request(string catalogVersion, string itemID, string currency, int price)
            {
                var request = CreateRequest();

                request.CatalogVersion = catalogVersion;
                request.ItemId = itemID;
                request.Price = price;
                request.VirtualCurrency = currency;

                Send(request);
            }

            public virtual void Request(CatalogItem item, string currency)
            {
                Request(item.CatalogVersion, item.ItemId, currency, (int)item.VirtualCurrencyPrices[currency]);
            }
        }

        public ExecuteCloudScriptHandler ExecuteCloudScript { get; protected set; }
        public class ExecuteCloudScriptHandler : PlayFabRequestHandler<ExecuteCloudScriptRequest, ExecuteCloudScriptResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.ExecuteCloudScript;

            public virtual void Request(string name, object parameter)
            {
                var request = CreateRequest();

                request.FunctionName = name;
                request.FunctionParameter = parameter;

                Send(request);
            }
        }

        public override void Configure()
        {
            base.Configure();

            GetCatalog = new GetCatalogHandler();

            GetUserInventory = new GetUserInventoryHandler();

            ListUserCharacters = new ListUserCharactersHandler();

            PurchaseItem = new PurchaseItemHandler();

            ExecuteCloudScript = new ExecuteCloudScriptHandler();
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