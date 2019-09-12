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
    public class PlayerInventoryCore : PlayerCore.Module
    {
        public List<ItemInstance> Items { get; protected set; }

        public Dictionary<string, int> VirtualCurrency { get; protected set; }

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

        public virtual bool CompliesWith(ItemRequirementData requirement)
        {
            if (requirement == null) return true;

            for (int i = 0; i < Items.Count; i++)
                if (Items[i].ItemId == requirement.Item.ID)
                    if (Items[i].RemainingUses >= requirement.Count)
                        return true;

            return false;
        }
        public virtual bool CompliesWith(ItemRequirementData[] requirements)
        {
            if (requirements == null) return true;

            for (int i = 0; i < requirements.Length; i++)
            {
                if (CompliesWith(requirements[i]))
                    continue;
                else
                    return false;
            }

            return true;
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

        #region Request
        public virtual void Request()
        {
            var request = new GetUserInventoryRequest
            {

            };

            PlayFabClientAPI.GetUserInventory(request, ResultCallback, ErrorCallback);
        }

        public delegate void ResultDelegate(PlayerInventoryCore inventory);
        public event ResultDelegate OnRetrieved;
        void ResultCallback(GetUserInventoryResult result)
        {
            Items = result.Inventory;

            VirtualCurrency = result.VirtualCurrency;

            if (OnRetrieved != null) OnRetrieved(this);

            Respond(null);
        }

        public delegate void ErrorDelegate(PlayFabError error);
        public event ErrorDelegate OnError;
        void ErrorCallback(PlayFabError error)
        {
            if (OnError != null) OnError(error);

            Respond(error);
        }

        public delegate void ResponseCallback(PlayerInventoryCore inventory, PlayFabError error);
        public event ResponseCallback OnResponse;
        void Respond(PlayFabError error)
        {
            if (OnResponse != null) OnResponse(this, error);
        }
        #endregion
    }
}