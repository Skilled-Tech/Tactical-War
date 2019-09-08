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

        public delegate void ResultDelegate(PlayFabPurchaseCore purchase, PurchaseItemResult result);
        public event ResultDelegate OnResult;
        void ResultCallback(PurchaseItemResult result)
        {
            if (OnResult != null) OnResult(this, result);

            Respond(result, null);
        }

        public delegate void ErrorDelegate(PlayFabError error);
        public event ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);

            Respond(null, error);
        }

        public delegate void ResponseCallback(PlayFabPurchaseCore purchase, PurchaseItemResult result, PlayFabError error);
        public event ResponseCallback OnResponse;
        void Respond(PurchaseItemResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, result, error);
        }
    }
}