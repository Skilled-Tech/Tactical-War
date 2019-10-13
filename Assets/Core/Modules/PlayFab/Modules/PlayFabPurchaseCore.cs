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
    public class PlayFabPurchaseCore : PlayFabCore.Property
    {
        public virtual void Perform(string itemID, int price, string currencyCode)
        {
            if(currencyCode == "RM")
            {
                Debug.LogError("Cannot buy Real Money item: " + itemID + " using the " + nameof(PlayFabPurchaseCore) + " Core, RM purchases must go through a different pipeline");
                return;
            }

            var request = new PurchaseItemRequest
            {
                CatalogVersion = PlayFab.Catalog.Version,
                ItemId = itemID,
                Price = price,
                VirtualCurrency = currencyCode,
            };

            PlayFabClientAPI.PurchaseItem(request, ResultCallback, ErrorCallback);
        }

        public virtual void Perform(CatalogItem item, string currencyCode)
        {
            Perform(item.ItemId, (int)item.VirtualCurrencyPrices[currencyCode], currencyCode);
        }
        public virtual void Perform(CatalogItem item)
        {
            var currencyCode = item.VirtualCurrencyPrices.First().Key;

            Perform(item, currencyCode);
        }
        
        public virtual void Perform(ItemTemplate item, string currencyCode)
        {
            Perform(item.ID, item.Price.Value, currencyCode);
        }
        public virtual void Perform(ItemTemplate item)
        {
            var currencyCode = CurrencyCode.From(item.Price.Type);

            Perform(item, currencyCode);
        }

        public event Delegates.ResultDelegate<PurchaseItemResult> OnResult;
        void ResultCallback(PurchaseItemResult result)
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

        public event Delegates.ResponseDelegate<PurchaseItemResult> OnResponse;
        void Respond(PurchaseItemResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(result, error);
        }
    }

    class C
    {
        void S()
        {
            var a = new StartPurchaseRequest
            {

            };

            var b = new PayForPurchaseRequest
            {

            };

            var c = new ConfirmPurchaseRequest
            {

            };
        }
    }
}