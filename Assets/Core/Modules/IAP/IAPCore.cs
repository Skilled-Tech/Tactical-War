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

        private IStoreController StoreController;
        private IGooglePlayStoreExtensions PlayStoreExtensions;

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
                public void Request(string currencyCode, uint purchasePrice, string reciept, string signature)
                {
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
                public void Request(Product product)
                {

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

            [Serializable]
            public class PurchaseData
            {
                public PurchaseData()
                {

                }
            }

            public override void Configure()
            {
                base.Configure();

                Register(validate);
            }
        }

        public bool Active
        {
            get
            {
                return StoreController != null;
            }
        }

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
            var store = AppStore.GooglePlay;

            store = AppStore.GooglePlay;

            var module = StandardPurchasingModule.Instance(store);

            var builder = ConfigurationBuilder.Instance(module);

            for (int i = 0; i < PlayFab.Catalog.Size; i++)
                builder.AddProduct(PlayFab.Catalog[i].ItemId, ProductType.Consumable);

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

        PurchaseProcessingResult ProcessPurchaseHandler(Product prodcut)
        {
            if (prodcut == null)
            {
                Debug.LogWarning("Attempted to process purchase with unknown product, ignoring");
                return PurchaseProcessingResult.Complete;
            }

            if (string.IsNullOrEmpty(prodcut.receipt))
            {
                Debug.LogWarning("Attempted to process purchase with no receipt, ignoring");
                return PurchaseProcessingResult.Complete;
            }

            Debug.Log("Processing purchase for item: " + prodcut.definition.storeSpecificId + " with transaction ID: " + prodcut.transactionID);

            return PurchaseProcessingResult.Complete;
        }
        void PurchaseFailedCallback(Product product, PurchaseFailureReason reason)
        {
            Debug.LogError("Purchase of prodcut " + product.definition.storeSpecificId + " failed, reason: " + reason);
        }

        public void Purchase(string productID)
        {
            if (Active == false) throw new Exception("IAP Core is not initialized");

            StoreController.InitiatePurchase(productID);
        }
    }
}