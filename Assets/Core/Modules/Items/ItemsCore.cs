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

namespace Game
{
    [Serializable]
	public class ItemsCore : Core.Module
	{
        #region List
        [SerializeField]
        protected ItemTemplate[] list;
        public ItemTemplate[] List { get { return list; } }

        public int Count { get { return list.Length; } }

        public ItemTemplate this[int index] { get { return list[index]; } }
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

            upgrades.Configure();
            units.Configure();
        }

        public override void Init()
        {
            base.Init();

            upgrades.Init();
            units.Init();
        }

        public virtual ItemTemplate Find(string itemID)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].ID == itemID)
                    return list[i];

            return null;
        }

        void OnCatalogRetrieved(PlayFabCatalogCore catalog)
        {
            for (int i = 0; i < list.Length; i++)
            {
                var catalogItem = catalog.Find(list[i].ID);

                if (catalogItem == null)
                {
                    Debug.LogWarning(list[i].name + " Has no item matching it's ID in the catalog recieved, ignoring");
                    continue;
                }

                list[i].Load(catalogItem);
            }
        }
    }
}