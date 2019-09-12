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
        public Dictionary<ItemTemplate, ItemUpgradeData> Dictionary { get; protected set; }

        public ItemsCore Items { get { return Core.Items; } }

        public ItemUpgradeData Find(ItemTemplate template)
        {
            if (Dictionary.ContainsKey(template) == false) return null;

            return Dictionary[template];
        }

        public override void Configure()
        {
            base.Configure();

            Dictionary = new Dictionary<ItemTemplate, ItemUpgradeData>();

            Player.Inventory.OnRetrieved += OnInventoryRetrieved;
        }

        void OnInventoryRetrieved(PlayerInventoryCore inventory)
        {
            Dictionary.Clear();

            for (int i = 0; i < inventory.Items.Count; i++)
            {
                var template = Items.Find(inventory.Items[i].ItemId);

                if (Items.Upgrades.IsUpgradable(template.CatalogItem))
                {
                    var data = new ItemUpgradeData(inventory.Items[i], template);

                    Dictionary.Add(template, data);
                }
            }
        }
    }

    [Serializable]
    public class ItemUpgradeData
    {
        public static Core Core { get { return Core.Instance; } }
        public static ItemsCore Items { get { return Core.Items; } }

        [JsonProperty]
        [SerializeField]
        protected List<ElementData> list;
        public List<ElementData> List { get { return list; } }
        [Serializable]
        public class ElementData
        {
            [JsonProperty]
            [JsonConverter(typeof(ItemUpgradeType.Converter))]
            [SerializeField]
            protected ItemUpgradeType type;
            public ItemUpgradeType Type { get { return type; } }

            [JsonProperty]
            [SerializeField]
            protected int value;
            public int Value { get { return value; } }

            public ElementData()
            {

            }

            public ElementData(ItemUpgradeType type)
            {
                this.type = type;

                this.value = 0;
            }
        }

        public virtual ElementData Find(ItemUpgradeType type)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].Type == type)
                    return list[i];

            return null;
        }

        void Load(string json)
        {
            JsonConvert.PopulateObject(json, list);
        }
        void Load(ItemInstance instance)
        {
            if (instance.CustomData == null)
            {
                list.Clear();
            }
            else
            {
                if (instance.CustomData.ContainsKey(ItemsUpgradesCore.Key))
                    Load(instance.CustomData[ItemsUpgradesCore.Key]);
                else
                    list.Clear();
            }
        }

        void CheckDefaults(ItemUpgradeType[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                var element = Find(types[i]);

                if (element == null)
                {
                    var instance = new ElementData(types[i]);

                    list.Add(instance);
                }
            }
        }

        public ItemUpgradeData(ItemInstance item, ItemTemplate template)
        {
            list = new List<ElementData>();

            Load(item);

            CheckDefaults(template.Upgrades.Applicable);
        }
    }
}