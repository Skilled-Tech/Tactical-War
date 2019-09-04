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
    [CreateAssetMenu(menuName = MenuPath + "Purchase")]
	public class PlayFabPurchaseCore : PlayFabCore.Module
	{
        public RequestHandler Request { get; protected set; }
		public class RequestHandler : PlayFabRequestHandler<PurchaseItemRequest, PurchaseItemResult>
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

        public event PlayFabRequestsUtility.ResponseDelegate<PurchaseItemResult> OnResponse;
        void ResponseCallback(PurchaseItemResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(result, error);
        }
    }
}