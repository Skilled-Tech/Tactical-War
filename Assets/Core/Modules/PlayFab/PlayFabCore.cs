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
    [CreateAssetMenu(menuName = MenuPath + "Asset")]
	public class PlayFabCore : Core.Module
	{
        new public const string MenuPath = Core.Module.MenuPath + "PlayFab/";

        [SerializeField]
        protected LoginCore login;
        public LoginCore Login { get { return login; } }
        [Serializable]
        public class LoginCore : PlayFabCore.Property
        {
            public EmaiLoginHandler EmailLogin { get; protected set; }
            public class EmaiLoginHandler : PlayFabCore.RequestHandler<LoginWithEmailAddressRequest, LoginResult>
            {
                public override AskDelegate Ask => PlayFabClientAPI.LoginWithEmailAddress;

                public virtual void Send(string email, string password)
                {
                    var request = CreateRequest();

                    request.Email = email;
                    request.Password = password;

                    Send(request);
                }
            }

            public AndroidLoginHandler AndroidIDLogin;
            public class AndroidLoginHandler : PlayFabCore.RequestHandler<LoginWithAndroidDeviceIDRequest, LoginResult>
            {
                public override AskDelegate Ask => PlayFabClientAPI.LoginWithAndroidDeviceID;

                public virtual void Send()
                {
                    var request = CreateRequest();

                    request.CreateAccount = true;

                    request.AndroidDeviceId = SystemInfo.deviceUniqueIdentifier;
                    request.AndroidDevice = SystemInfo.deviceModel;
                    request.OS = SystemInfo.operatingSystem;

                    Send(request);
                }
            }

            public override void Configure()
            {
                base.Configure();

                EmailLogin = new EmaiLoginHandler();
                EmailLogin.OnResponse += ResponseCallback;

                AndroidIDLogin = new AndroidLoginHandler();
                AndroidIDLogin.OnResponse += ResponseCallback;
            }

            public virtual void Perform()
            {
                if (Application.isEditor)
                {
                    EmailLogin.Send("Moe4Baker@gmail.com", "Password");
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    AndroidIDLogin.Send();
                }
                else
                {

                }
            }

            public event PlayFabCore.Utility.ResponseDelegate<LoginCore> OnResponse;
            void ResponseCallback(LoginResult result, PlayFabError error)
            {
                if (OnResponse != null) OnResponse(this, error);
            }
        }

        [SerializeField]
        protected TitleCore title;
        public TitleCore Title { get { return title; } }
        [Serializable]
        public class TitleCore : PlayFabCore.Property
        {
            [SerializeField]
            protected DataCore data;
            public DataCore Data { get { return data; } }
            [Serializable]
            public class DataCore
            {
                public GetRequestHandler GetRequest { get; protected set; }
                public class GetRequestHandler : PlayFabCore.RequestHandler<GetTitleDataRequest, GetTitleDataResult>
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

                public event PlayFabCore.Utility.ResponseDelegate<DataCore> OnResponse;
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

            public event PlayFabCore.Utility.ResponseDelegate<TitleCore> OnResponse;
            public virtual void ResponseCompleted(PlayFabError error)
            {
                if (OnResponse != null) OnResponse(this, error);
            }
        }

        [SerializeField]
        protected CatalogsCore catalogs;
        public CatalogsCore Catalogs { get { return catalogs; } }
        [Serializable]
        public class CatalogsCore : PlayFabCore.Property
        {
            [SerializeField]
            protected PlayFabCatalog[] elements;
            public PlayFabCatalog[] Elements { get { return elements; } }

            public class Module : PlayFabCore.Module
            {
                public CatalogsCore Catalogs { get { return PlayFab.Catalogs; } }
            }

            public override void Configure()
            {
                base.Configure();

                for (int i = 0; i < elements.Length; i++)
                {
                    elements[i].Configure();

                    elements[i].OnRetrieved += OnElementRetrieved;
                    elements[i].OnResponse += OnElementResponse;
                }
            }

            public override void Init()
            {
                base.Init();

                for (int i = 0; i < elements.Length; i++)
                    elements[i].Init();
            }

            public virtual void Request()
            {
                for (int i = 0; i < elements.Length; i++)
                    elements[i].Request();
            }

            public event PlayFabCore.Utility.ResaultDelegate<CatalogsCore> OnResult;
            void OnElementRetrieved(PlayFabCatalog result)
            {
                if (elements.All(x => x.Valid))
                    if (OnResult != null) OnResult(this);
            }

            public event PlayFabCore.Utility.ResponseDelegate<CatalogsCore> OnResponse;
            void OnElementResponse(PlayFabCatalog result, PlayFabError error)
            {
                if (error == null)
                {
                    if (elements.All(x => x.Valid))
                        if (OnResponse != null) OnResponse(this, error);
                }
                else
                {
                    if (OnResponse != null) OnResponse(this, error);
                }
            }
        }

        [SerializeField]
        protected InventoryCore inventory;
        public InventoryCore Inventory { get { return inventory; } }
        [Serializable]
        public class InventoryCore : PlayFabCore.Property
        {
            public RequestHandler GetRequest { get; protected set; }
            public class RequestHandler : PlayFabCore.RequestHandler<GetUserInventoryRequest, GetUserInventoryResult>
            {
                public override AskDelegate Ask => PlayFabClientAPI.GetUserInventory;

                public virtual void Send()
                {
                    var request = CreateRequest();

                    Send(request);
                }
            }

            public List<ItemInstance> Items
            {
                get
                {
                    if (GetRequest.Latest == null) return null;

                    return GetRequest.Latest.Inventory;
                }
            }

            public Dictionary<string, int> Currencies
            {
                get
                {
                    if (GetRequest.Latest == null) return null;

                    return GetRequest.Latest.VirtualCurrency;
                }
            }

            public override void Configure()
            {
                base.Configure();

                GetRequest = new RequestHandler();

                GetRequest.OnResponse += ResponseCallback;

                GetRequest.OnResult += RetrieveCallback;
            }

            public virtual bool Contains(CatalogItem item)
            {
                return Contains(item.ItemId);
            }
            public virtual bool Contains(string itemID)
            {
                for (int i = 0; i < Items.Count; i++)
                    if (Items[i].ItemId == itemID)
                        return true;

                return false;
            }

            public virtual ItemInstance FindInstance(string itemID)
            {
                if(Items != null)
                {
                    for (int i = 0; i < Items.Count; i++)
                        if (Items[i].ItemId == itemID)
                            return Items[i];
                }

                return null;
            }

            public virtual void Request()
            {
                GetRequest.Send();
            }

            public event PlayFabCore.Utility.ResponseDelegate<InventoryCore> OnResponse;
            void ResponseCallback(GetUserInventoryResult result, PlayFabError error)
            {
                if (OnResponse != null) OnResponse(this, error);
            }

            public event PlayFabCore.Utility.ResaultDelegate<InventoryCore> OnRetrieved;
            void RetrieveCallback(GetUserInventoryResult result)
            {
                if (OnRetrieved != null) OnRetrieved(this);
            }
        }

        [SerializeField]
        protected PurchaseCore purchase;
        public PurchaseCore Purchase { get { return purchase; } }
        [Serializable]
        public class PurchaseCore : PlayFabCore.Property
        {
            public RequestHandler Request { get; protected set; }
            public class RequestHandler : PlayFabCore.RequestHandler<PurchaseItemRequest, PurchaseItemResult>
            {
                public override AskDelegate Ask => PlayFabClientAPI.PurchaseItem;

                public virtual void Send(CatalogItem item, string currency)
                {
                    var request = CreateRequest();

                    request.CatalogVersion = item.CatalogVersion;
                    request.ItemId = item.ItemId;
                    request.Price = (int)item.VirtualCurrencyPrices[currency];
                    request.VirtualCurrency = currency;

                    Send(request);
                }
            }

            public override void Configure()
            {
                base.Configure();

                Request = new RequestHandler();
                Request.OnResponse += ResponseCallback;
            }

            public virtual void Perform(CatalogItem item, string currency)
            {
                Request.Send(item, currency);
            }

            public event PlayFabCore.Utility.ResponseDelegate<PurchaseItemResult> OnResponse;
            void ResponseCallback(PurchaseItemResult result, PlayFabError error)
            {
                if (OnResponse != null) OnResponse(result, error);
            }
        }

        [SerializeField]
        protected UpgradeCore upgrade;
        public UpgradeCore Upgrade { get { return upgrade; } }
        [Serializable]
        public class UpgradeCore : Property
        {
            public RequestHandler Request { get; protected set; }
            public class RequestHandler : RequestHandler<ExecuteCloudScriptRequest, ExecuteCloudScriptResult>
            {
                public override AskDelegate Ask => PlayFabClientAPI.ExecuteCloudScript;

                public virtual void Send(string instanceID, string type)
                {
                    var request = CreateRequest();

                    request.FunctionName = "UpgradeItem";

                    request.FunctionParameter = new
                    {
                        ItemInstanceId = instanceID,
                        UpgradeType = type,
                    };

                    request.GeneratePlayStreamEvent = true;

                    Send(request);
                }
            }

            public override void Configure()
            {
                base.Configure();

                Request = new RequestHandler();

                Request.OnResponse += ResponseCallback;
            }

            public virtual void Perform(string itemInstanceID, string type)
            {
                Request.Send(itemInstanceID, type);
            }

            public event Utility.ResponseDelegate<ExecuteCloudScriptResult> OnResponse;
            void ResponseCallback(ExecuteCloudScriptResult result, PlayFabError error)
            {
                if (OnResponse != null) OnResponse(result, error);
            }
        }

        [Serializable]
        public class Property : Core.Property
        {
            public PlayFabCore PlayFab { get { return Core.PlayFab; } }
        }

        public class Module : Core.Module
        {
            public PlayFabCore PlayFab { get { return Core.PlayFab; } }

            new public const string MenuPath = PlayFabCore.MenuPath + "Modules/";
        }

        public virtual void ForAllElements(Action<Core.IElement> action)
        {
            action(login);
            action(title);
            action(catalogs);
            action(inventory);
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