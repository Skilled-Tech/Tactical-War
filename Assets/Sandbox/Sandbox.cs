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
    public class Sandbox : MonoBehaviour
    {
        #region Requests
        EmailLoginHandler EmailLogin;
        public class EmailLoginHandler : RequestHandler<LoginWithEmailAddressRequest, LoginResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.LoginWithEmailAddress;

            public virtual void Request(string email, string password)
            {
                var request = new LoginWithEmailAddressRequest
                {
                    Email = email,
                    Password = password
                };

                Send(request);
            }
        }

        GetCatalogHandler GetCatalog;
        public class GetCatalogHandler : RequestHandler<GetCatalogItemsRequest, GetCatalogItemsResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.GetCatalogItems;

            public virtual void Request(string version)
            {
                var request = new GetCatalogItemsRequest
                {
                    CatalogVersion = version
                };

                Send(request);
            }
        }

        GetUserInventoryHandler GetUserInventory;
        public class GetUserInventoryHandler : RequestHandler<GetUserInventoryRequest, GetUserInventoryResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.GetUserInventory;

            public virtual void Request()
            {

            }
        }

        PurchaseItemHandler PurchaseItem;
        public class PurchaseItemHandler : RequestHandler<PurchaseItemRequest, PurchaseItemResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.PurchaseItem;

            public virtual void Request(string catalogVersion, string itemID, string currency, int price)
            {
                var request = new PurchaseItemRequest
                {
                    CatalogVersion = catalogVersion,
                    ItemId = itemID,
                    Price = price,
                    VirtualCurrency = currency
                };

                Send(request);
            }
        }

        public abstract class RequestHandler<TRequest, TResult>
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
        #endregion

        void Awake()
        {
            EmailLogin = new EmailLoginHandler();

            GetCatalog = new GetCatalogHandler();

            GetUserInventory = new GetUserInventoryHandler();

            PurchaseItem = new PurchaseItemHandler();
        }

        void Start()
        {
            EmailLogin.OnResult += OnLogin;

            EmailLogin.Request("Moe4Baker@gmail.com", "Password");
        }

        void OnLogin(LoginResult result)
        {
            Debug.Log("Successful Login");

            GetCatalog.OnResult += OnCatalog;
            GetCatalog.Request("Units");
        }

        void OnCatalog(GetCatalogItemsResult result)
        {
            Debug.Log("Recieved Catalog");

            foreach (var item in result.Catalog)
            {
                Debug.Log(item.DisplayName);
            }

            GetUserInventory.OnResult += OnUserInventory;
            GetUserInventory.Request();
        }

        void OnUserInventory(GetUserInventoryResult result)
        {
            Debug.Log("Got User Inventory");

            foreach (var item in result.Inventory)
            {
                Debug.Log(item.DisplayName);
            }

            if (result.Inventory.Count == 0)
            {

            }
            else
            {

            }
        }

        void OnError(PlayFabError error)
        {

        }
    }
}