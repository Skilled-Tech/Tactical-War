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

using UnityEngine.Purchasing;

using PlayFab;
using PlayFab.ClientModels;

namespace Game
{
    [Serializable]
	public class IAPCore : Core.Property
    {
		public static PlayFabCore PlayFab { get { return Core.PlayFab; } }

        public AppStore Store { get; protected set; }
        public IStoreController StoreController { get; protected set; }
        public IGooglePlayStoreExtensions PlayStoreExtensions { get; protected set; }

        public bool Active
        {
            get
            {
                return StoreController != null;
            }
        }

        public ListenerCore Listener { get; protected set; }
        public class ListenerCore : Core.Property, IStoreListener
        {
            public delegate void InitializeDelegate(IStoreController controller, IExtensionProvider extensions);
            public event InitializeDelegate InitializeEvent;
            void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
            {
                if (InitializeEvent != null) InitializeEvent(controller, extensions);
            }


            public delegate void InitializeFailDelegate(InitializationFailureReason reason);
            public event InitializeFailDelegate InitializeFailEvent;
            void IStoreListener.OnInitializeFailed(InitializationFailureReason reason)
            {
                if (InitializeFailEvent != null) InitializeFailEvent(reason);
            }


            public delegate void PurchasePrcoessEventDelegate(Product product);
            public event PurchasePrcoessEventDelegate PurchaseProcessEvent;

            public delegate PurchaseProcessingResult PurchasePrcoessHandlerDelegate(Product product);
            public PurchasePrcoessHandlerDelegate PurchaseProcessHandler;

            PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs args)
            {
                if (PurchaseProcessEvent != null) PurchaseProcessEvent(args.purchasedProduct);

                return PurchaseProcessHandler(args.purchasedProduct);
            }


            public delegate void PurchaseFailDelegate(Product product, PurchaseFailureReason reason);
            public event PurchaseFailDelegate PurchaseFailEvent;
            void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason reason)
            {
                if (PurchaseFailEvent != null) PurchaseFailEvent(product, reason);
            }

            public ListenerCore(PurchasePrcoessHandlerDelegate PurchaseHandler)
            {
                this.PurchaseProcessHandler = PurchaseHandler;
            }
        }

        [SerializeField]
        protected GoogleCore google;
        public GoogleCore Google { get { return google; } }
        [Serializable]
        public class GoogleCore : Core.Property
        {
            [SerializeField]
            protected ValidateCore validate;
            public ValidateCore Validate { get { return validate; } }

            [Serializable]
            public class ValidateCore : PlayFabCore.Property
            {
                public void Request(Product product)
                {
                    var reciept = GooglePurchase.FromJson(product.receipt);

                    var currencyCode = product.metadata.isoCurrencyCode;
                    var purchaseprice = (uint)product.metadata.localizedPrice * 100;
                    var recieptJson = reciept.PayloadData.json;
                    var signature = reciept.PayloadData.signature;

                    Request(currencyCode, purchaseprice, recieptJson, signature);
                }
                public void Request(string currencyCode, uint purchasePrice, string reciept, string signature)
                {
                    Debug.Log("Validating " + Environment.NewLine + reciept + Environment.NewLine + signature);

                    var request = new ValidateGooglePlayPurchaseRequest
                    {
                        CatalogVersion = PlayFab.Catalog.Version,
                        CurrencyCode = currencyCode,
                        PurchasePrice = purchasePrice,
                        ReceiptJson = reciept,
                        Signature = signature,
                    };

                    PlayFabClientAPI.ValidateGooglePlayPurchase(request, RetrieveCallback, ErrorCallback);
                }

                public event Delegates.RetrievedDelegate<ValidateGooglePlayPurchaseResult> OnRetrieved;
                void RetrieveCallback(ValidateGooglePlayPurchaseResult result)
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

                public event Delegates.ResponseDelegate<ValidateGooglePlayPurchaseResult> OnResponse;
                void Respond(ValidateGooglePlayPurchaseResult result, PlayFabError error)
                {
                    if (OnResponse != null) OnResponse(result, error);
                }
            }

            public class JsonData
            {
                // Json Fields, ! Case-sensetive

                public string orderId;
                public string packageName;
                public string productId;
                public long purchaseTime;
                public int purchaseState;
                public string purchaseToken;
            }
            public class PayloadData
            {
                public JsonData JsonData;

                // Json Fields, ! Case-sensetive
                public string signature;
                public string json;

                public static PayloadData FromJson(string json)
                {
                    var payload = JsonUtility.FromJson<PayloadData>(json);
                    payload.JsonData = JsonUtility.FromJson<JsonData>(payload.json);
                    return payload;
                }
            }
            public class GooglePurchase
            {
                public PayloadData PayloadData;

                // Json Fields, ! Case-sensetive
                public string Store;
                public string TransactionID;
                public string Payload;

                public static GooglePurchase FromJson(string json)
                {
                    var purchase = JsonUtility.FromJson<GooglePurchase>(json);
                    purchase.PayloadData = PayloadData.FromJson(purchase.Payload);
                    return purchase;
                }
            }

            public override void Configure()
            {
                base.Configure();

                Register(validate);
            }
        }

        public PopupUI Popup => Core.UI.Popup;

        public override void Configure()
        {
            base.Configure();

            Listener = new ListenerCore(ProcessPurchaseHandler);
            Listener.InitializeEvent += InitializeCallback;
            Listener.InitializeFailEvent += InitializeFailedCallback;
            Listener.PurchaseFailEvent += PurchaseFailedCallback;

            Register(Listener);
            Register(google);

            PlayFab.Catalog.OnRetrieved += CatalogRetrievedCallback;
        }

        void CatalogRetrievedCallback(PlayFabCatalogCore catalog)
        {
            Initialize();
        }

        void Initialize()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    Store = AppStore.GooglePlay;
                    break;

                case RuntimePlatform.IPhonePlayer:
                    Store = AppStore.AppleAppStore;
                    break;

                default:
                    Store = AppStore.NotSpecified;
                    break;
            }

            var module = StandardPurchasingModule.Instance(Store);

            var builder = ConfigurationBuilder.Instance(module);

            for (int i = 0; i < PlayFab.Catalog.Size; i++)
            {
                if (PlayFab.Catalog[i].VirtualCurrencyPrices.ContainsKey("RM") == false) continue;

                builder.AddProduct(PlayFab.Catalog[i].ItemId, ProductType.Consumable);
            }

            UnityPurchasing.Initialize(Listener, builder);
        }

        void InitializeCallback(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("IAP Initialzed Correctly");

            StoreController = controller;

            PlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
        }
        void InitializeFailedCallback(InitializationFailureReason error)
        {
            Debug.LogError("Store initialization failed, reason: " + error);
        }

        public void Purchase(string productID)
        {
            if (Active == false) throw new Exception("IAP Core is not initialized");

            StoreController.InitiatePurchase(productID);
        }

        PurchaseProcessingResult ProcessPurchaseHandler(Product product)
        {
            if (product == null)
            {
                Debug.LogWarning("Attempted to process purchase with unknown product, ignoring");
                return PurchaseProcessingResult.Complete;
            }

            if (string.IsNullOrEmpty(product.receipt))
            {
                Debug.LogWarning("Attempted to process purchase with no receipt, ignoring");
                return PurchaseProcessingResult.Complete;
            }

            Debug.Log("Processing purchase for item: " + product.definition.storeSpecificId + " with transaction ID: " + product.transactionID);

            switch (Store)
            {
                case AppStore.GooglePlay:
                    Google.Validate.OnResponse += GoogleValidateResponseCallback;
                    Google.Validate.Request(product);
                    break;

                case AppStore.AppleAppStore: //TODO implement Apple App Store Purchase Process

                default:
                    Debug.LogError("IAP Purchase Process not implemented for " + Application.platform);
                    ErrorCallback("Not Implemented On Platform");
                    break;
            }

            return PurchaseProcessingResult.Complete;
        }

        #region Validate
        void GoogleValidateResponseCallback(ValidateGooglePlayPurchaseResult result, PlayFabError error)
        {
            Google.Validate.OnResponse -= GoogleValidateResponseCallback;

            if (error == null)
            {
                Debug.Log("Successfully Validated IAP Purchase");

                ValidateAction();
            }
            else
            {
                Debug.LogError("Error Validating IAP, report: " + error.GenerateErrorReport());

                ErrorCallback(error.ErrorMessage);
            }
        }

        public delegate void ValidateDelegate();
        public event ValidateDelegate OnValidate;
        void ValidateAction()
        {
            OnValidate?.Invoke();

            ResponseAction(null);
        }
        #endregion
        
        void PurchaseFailedCallback(Product product, PurchaseFailureReason reason)
        {
            Debug.LogError("Purchase of prodcut " + product.definition.storeSpecificId + " failed, reason: " + reason);

            if (reason == PurchaseFailureReason.UserCancelled) return;

            ErrorCallback(Tools.Text.AddSpacesToCamelCase(reason.ToString()));
        }

        public event Action<string> OnError;
        void ErrorCallback(string error)
        {
            if (OnError != null) OnError(error);

            ResponseAction(error);
        }

        public delegate void ResponseDelegate(string error);
        public event ResponseDelegate OnResponse;
        void ResponseAction(string error)
        {
            OnResponse?.Invoke(error);
        }
    }
}