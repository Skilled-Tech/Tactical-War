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
        public List<ItemInstance> Items { get; protected set; }

        public Dictionary<string, int> Balances { get; protected set; }

        public virtual void Request()
        {
            var request = new GetUserInventoryRequest
            {

            };

            PlayFabClientAPI.GetUserInventory(request, Retrieved, Error);
        }

        public event Action<PlayFabInventoryCore> OnRetrieved;
        public virtual void Retrieved(GetUserInventoryResult result)
        {
            Debug.Log("Retrieved Inventory");

            Items = result.Inventory;
            Balances = result.VirtualCurrency;

            if (OnRetrieved != null) OnRetrieved(this);
        }

        public virtual void Error(PlayFabError error)
        {

        }
    }
}