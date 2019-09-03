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
        EmailLoginHandler EmailLogin;
        public class EmailLoginHandler : Handler<LoginWithEmailAddressRequest, LoginResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.LoginWithEmailAddress;

            public virtual void Request(string email, string password)
            {
                var request = GetRequest();

                request.Email = email;
                request.Password = password;

                Send(request);
            }
        }

        GetCatalogHandler GetCatalog;
        public class GetCatalogHandler : Handler<GetCatalogItemsRequest, GetCatalogItemsResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.GetCatalogItems;

            public virtual void Request(string version)
            {
                var request = GetRequest();

                request.CatalogVersion = version;

                Send(request);
            }
        }

        GetUserInventoryHandler GetUserInventory;
        public class GetUserInventoryHandler : Handler<GetUserInventoryRequest, GetUserInventoryResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.GetUserInventory;

            public virtual void Request()
            {
                var request = GetRequest();

                Send(request);
            }
        }

        ListUserCharactersHandler ListUserCharacters;
        public class ListUserCharactersHandler : Handler<ListUsersCharactersRequest, ListUsersCharactersResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.GetAllUsersCharacters;

            public virtual void Request()
            {
                var request = GetRequest();

                Send(request);
            }
        }

        PurchaseItemHandler PurchaseItem;
        public class PurchaseItemHandler : Handler<PurchaseItemRequest, PurchaseItemResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.PurchaseItem;

            public virtual void Request(string catalogVersion, string itemID, string currency, int price)
            {
                var request = GetRequest();

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

        ExecuteCloudScriptHandler ExecuteCloudScript;
        public class ExecuteCloudScriptHandler : Handler<ExecuteCloudScriptRequest, ExecuteCloudScriptResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.ExecuteCloudScript;

            public virtual void Request(string name, object parameter)
            {
                var request = GetRequest();

                request.FunctionName = name;
                request.FunctionParameter = parameter;

                Send(request);
            }
        }

        public abstract class Handler<TRequest, TResult>
        where TRequest : class, new()
        where TResult : class
        {
            public TResult Latest { get; protected set; }

            public delegate void AskDelegate(TRequest request, Action<TResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null);
            public abstract AskDelegate Ask { get; }

            public virtual void Send(TRequest request)
            {
                Ask(request, ResultCallback, ErrorCallback);
            }

            public virtual TRequest GetRequest()
            {
                return new TRequest();
            }

            public delegate void ResaultDelegate(TResult result);
            public event ResaultDelegate OnResult;
            public virtual void ResultCallback(TResult result)
            {
                Latest = result;

                if (OnResult != null) OnResult(result);
            }

            public delegate void ErrorDelegate(PlayFabError error);
            public event ErrorDelegate OnError;
            public virtual void ErrorCallback(PlayFabError error)
            {
                Debug.LogError("Error On Request, Report: " + error.GenerateErrorReport());

                if (OnError != null) OnError(error);
            }
        }

        public override void Configure()
        {
            base.Configure();

            EmailLogin = new EmailLoginHandler();

            GetCatalog = new GetCatalogHandler();

            GetUserInventory = new GetUserInventoryHandler();

            ListUserCharacters = new ListUserCharactersHandler();

            PurchaseItem = new PurchaseItemHandler();

            ExecuteCloudScript = new ExecuteCloudScriptHandler();
        }
    }
}