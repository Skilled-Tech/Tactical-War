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

using Newtonsoft.Json;

namespace Game
{
    [Serializable]
    public class PlayFabPlayerInventoryCore : PlayFabPlayerCore.Module
    {
        public List<ItemInstance> Items { get; protected set; }

        public Dictionary<string, int> VirtualCurrency { get; protected set; }

        public const string FileName = "Inventory";

        #region Query
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
        public virtual bool Contains(string itemID, int count)
        {
            for (int i = 0; i < Items.Count; i++)
                if (Items[i].ItemId == itemID)
                    if (Items[i].RemainingUses >= count)
                        return true;

            return false;
        }

        public virtual ItemInstance Find(CatalogItem item)
        {
            if (item == null) return null;

            return Find(item.ItemId);
        }
        public virtual ItemInstance Find(string itemID)
        {
            for (int i = 0; i < Items.Count; i++)
                if (Items[i].ItemId == itemID)
                    return Items[i];

            return null;
        }
        #endregion

        #region Request
        public virtual void Request()
        {
            if (IsOnline)
            {
                var request = new GetUserInventoryRequest
                {

                };

                PlayFabClientAPI.GetUserInventory(request, RetrieveCallback, ErrorCallback);
            }
            else
                Load<GetUserInventoryResult>(FileName, RetrieveCallback, ErrorCallback);
        }

        public event Delegates.RetrievedDelegate<PlayFabPlayerInventoryCore> OnRetrieved;
        void RetrieveCallback(GetUserInventoryResult result)
        {
            this.Items = result.Inventory;
            this.VirtualCurrency = result.VirtualCurrency;

            if (OnRetrieved != null) OnRetrieved(this);

            Respond(null);

            if (IsOnline)
                Save(result, FileName);
        }

        public event Delegates.ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);

            Respond(error);
        }

        public event Delegates.ResponseDelegate<PlayFabPlayerInventoryCore> OnResponse;
        void Respond(PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);
        }
        #endregion
    }
}