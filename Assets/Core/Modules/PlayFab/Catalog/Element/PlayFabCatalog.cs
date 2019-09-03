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
    [CreateAssetMenu(menuName = MenuPath + "Catalog")]
    public class PlayFabCatalog : PlayFabCatalogsCore.Module
	{
        public virtual string Version { get { return name; } }

        public List<CatalogItem> Items { get; protected set; }

        public virtual bool Valid { get { return Items != null; } }

        public int Size { get { return Items.Count; } }
        public CatalogItem this[int index] { get { return Items[index]; } }

        public virtual void Request()
        {
            Items = null;

            var request = new GetCatalogItemsRequest
            {
                CatalogVersion = Version,
            };

            PlayFabClientAPI.GetCatalogItems(request, Retrieved, OnError);
        }

        public event Action<PlayFabCatalog> OnRetrieved;
        public virtual void Retrieved(GetCatalogItemsResult result)
        {
            Debug.Log("Retrieved " + Version + " Catalog");

            Items = result.Catalog;

            if (OnRetrieved != null) OnRetrieved(this);
        }

        public virtual void OnError(PlayFabError error)
        {
            
        }
    }
}