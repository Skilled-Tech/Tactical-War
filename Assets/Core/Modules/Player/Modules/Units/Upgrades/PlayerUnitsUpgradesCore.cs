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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PlayFab;
using PlayFab.ClientModels;

namespace Game
{
    [Serializable]
    public class PlayerUnitsUpgradesCore : PlayerUnitsCore.Module
	{
        public Dictionary<string, UnitUpgradeData> Dictionary { get; protected set; }

        public List<UnitUpgradeData> List;

        public UnitUpgradeData Find(UnitTemplate template)
        {
            if (Dictionary.ContainsKey(template.CatalogItem.ItemId) == false) return null;

            return Dictionary[template.CatalogItem.ItemId];
        }

        public override void Configure()
        {
            base.Configure();

            Dictionary = new Dictionary<string, UnitUpgradeData>();

            Player.Inventory.OnRetrieved += OnInventoryRetrieved;
        }

        void OnInventoryRetrieved(PlayerInventoryCore inventory)
        {
            Dictionary.Clear();

            foreach (var item in inventory.Items)
            {
                Dictionary.Add(item.ItemId, new UnitUpgradeData(item));
            }
        }
    }

    [Serializable]
    public class UnitUpgradeData
    {
        public static Core Core { get { return Core.Instance; } }

        [SerializeField]
        protected ElementData[] list;
        public ElementData[] List { get { return list; } }
        [Serializable]
        public class ElementData
        {
            [SerializeField]
            protected ItemUpgradeType type;
            public ItemUpgradeType Type { get { return type; } }

            [SerializeField]
            protected int value;
            public int Value { get { return value; } }

            public ElementData(JToken token)
            {
                type = Core.Items.Upgrades.Types.Find(token[nameof(Type)]);

                value = token[nameof(Value)].ToObject<int>();
            }
        }

        public virtual ElementData Find(ItemUpgradeType type)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].Type == type)
                    return list[i];

            return null;
        }

        public void Load(JArray array)
        {
            list = new ElementData[array.Count];

            for (int i = 0; i < array.Count; i++)
            {
                list[i] = new ElementData(array[i]);
            }
        }
        public void Load(string json)
        {
            Load(JArray.Parse(json));
        }
        public void Load(ItemInstance item)
        {
            if (item.CustomData == null)
            {
                list = new ElementData[] { };
            }
            else
            {
                if(item.CustomData.ContainsKey(ItemsUpgradesCore.Key))
                {
                    Load(item.CustomData[ItemsUpgradesCore.Key]);
                }
                else
                {
                    list = new ElementData[] { };
                }
            }
        }

        public UnitUpgradeData()
        {
            list = new ElementData[] { };
        }
        public UnitUpgradeData(ItemInstance item) : this()
        {
            Load(item);
        }
    }
}