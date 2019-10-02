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

using PlayFab.ClientModels;

namespace Game
{
    [Serializable]
	public class ItemsCore : Core.Module
	{
        #region List
        [SerializeField]
        protected ItemsRoster roster;
        public ItemsRoster Roster { get { return roster; } }

        public IList<ItemTemplate> List { get { return roster.List; } }

        public int Count { get { return List.Count; } }

        public ItemTemplate this[int index] { get { return List[index]; } }
        #endregion

        [SerializeField]
        protected ItemsUnitsCore units;
        public ItemsUnitsCore Units { get { return units; } }

        [SerializeField]
        protected ItemsUpgradesCore upgrades;
        public ItemsUpgradesCore Upgrades { get { return upgrades; } }

        [Serializable]
        public class Module : Core.Module
        {
            public ItemsCore Items { get { return Core.Items; } }
        }

        public override void Configure()
        {
            base.Configure();

            Core.PlayFab.Catalog.OnRetrieved += OnCatalogRetrieved;

            Register(upgrades);
            Register(units);
        }

        public virtual ItemTemplate Find(CatalogItem item)
        {
            return Find(item.ItemId);
        }
        public virtual ItemTemplate Find(string itemID)
        {
            for (int i = 0; i < List.Count; i++)
                if (List[i].ID == itemID)
                    return List[i];

            return null;
        }

        void OnCatalogRetrieved(PlayFabCatalogCore catalog)
        {
            for (int i = 0; i < List.Count; i++)
            {
                var catalogItem = catalog.Find(List[i].ID);

                if (catalogItem == null)
                {
                    Debug.LogWarning(List[i].name + " Has no item matching it's ID in the catalog recieved, ignoring");
                    continue;
                }

                List[i].Load(catalogItem);
            }
        }
    }
}