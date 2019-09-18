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
    public class PlayFabPurchaseCore : PlayFabCore.Module
    {
        public virtual void Perform(string itemID, int price, string currency)
        {
            var request = new PurchaseItemRequest
            {
                CatalogVersion = PlayFabCatalogCore.Version,
                ItemId = itemID,
                Price = price,
                VirtualCurrency = currency,
            };

            PlayFabClientAPI.PurchaseItem(request, ResultCallback, ErrorCallback);
        }
        public virtual void Perform(CatalogItem item, string currency)
        {
            Perform(item.ItemId, (int)item.VirtualCurrencyPrices[currency], currency);
        }
        public void Perform(CatalogItem item)
        {
            var currency = item.VirtualCurrencyPrices.First().Key;

            Perform(item, currency);
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
}