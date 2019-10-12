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

namespace Game
{
    [Serializable]
	public class IAPCore : Core.Property, IStoreListener
    {
		public PlayFabCore PlayFab { get { return Core.PlayFab; } }

        private IStoreController StoreController;
        private IGooglePlayStoreExtensions PlayStoreExtensions;

        public bool Initialized
        {
            get
            {
                return StoreController != null;
            }
        }

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("IAP Initialzed Correctly");

            StoreController = controller;

            PlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
        }
        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError("Store initialization failed, reason: " + error);
        }

        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs args)
        {
            Debug.Log("Processing purchase for " + args.purchasedProduct.definition.storeSpecificId);

            return PurchaseProcessingResult.Complete;
        }
        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            Debug.LogError("Purchase of prodcut " + product.definition.storeSpecificId + " failed, reason: " + reason);
        }

        public override void Configure()
        {
            base.Configure();

            PlayFab.Catalog.OnRetrieved += CatalogRetrievedCallback;
        }

        void CatalogRetrievedCallback(PlayFabCatalogCore catalog)
        {
            Initialize();
        }

        void Initialize()
        {
            AppStore store = AppStore.GooglePlay;

            var module = StandardPurchasingModule.Instance(store);

            var builder = ConfigurationBuilder.Instance(module);

            for (int i = 0; i < PlayFab.Catalog.Size; i++)
                builder.AddProduct(PlayFab.Catalog[i].ItemId, ProductType.Consumable);

            UnityPurchasing.Initialize(this, builder);
        }

        public void Purchase(string productID)
        {
            if (Initialized == false) throw new Exception("IAP Core is not initialized");

            StoreController.InitiatePurchase(productID);
        }
    }
}