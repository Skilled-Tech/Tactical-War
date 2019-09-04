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
    [CreateAssetMenu(menuName = MenuPath + "Inventory")]
	public class PlayFabInventoryCore : PlayFabCore.Module
	{
        public RequestHandler GetRequest { get; protected set; }
        public class RequestHandler : PlayFabRequestHandler<GetUserInventoryRequest, GetUserInventoryResult>
        {
            public override AskDelegate Ask => PlayFabClientAPI.GetUserInventory;

            public virtual void Send()
            {
                var request = CreateRequest();

                Send(request);
            }
        }

        public List<ItemInstance> Items { get { return GetRequest.Latest.Inventory; } }

        public Dictionary<string, int> Currencies { get { return GetRequest.Latest.VirtualCurrency; } }

        public override void Configure()
        {
            base.Configure();

            GetRequest = new RequestHandler();

            GetRequest.OnResponse += ResponseCallback;

            GetRequest.OnResult += RetrieveCallback;
        }

        public virtual bool Contains(CatalogItem item)
        {
            for (int i = 0; i < Items.Count; i++)
                if (Items[i].ItemId == item.ItemId)
                    return true;

            return false;
        }

        public virtual void Request()
        {
            GetRequest.Send();
        }

        public event PlayFabRequestsUtility.ResponseDelegate<PlayFabInventoryCore> OnResponse;
        void ResponseCallback(GetUserInventoryResult result, PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);
        }

        public event PlayFabRequestsUtility.ResaultDelegate<PlayFabInventoryCore> OnRetrieved;
        void RetrieveCallback(GetUserInventoryResult result)
        {
            if (OnRetrieved != null) OnRetrieved(this);
        }
    }
}