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

        public PlayerInventoryCore Inventory { get { return Player.Inventory; } }

        public override void Configure()
        {
            base.Configure();

            Dictionary = new Dictionary<ItemTemplate, ItemUpgradeData>();

            Player.Inventory.OnUpdate += OnInventoryChanged;
        }

        void OnInventoryChanged()
        {
            Dictionary.Clear();

            for (int i = 0; i < Inventory.Items.Count; i++)
            {
                if (Items.Upgrades.IsUpgradable(Inventory.Items[i].Template.CatalogItem))
                {
                    var data = new ItemUpgradeData(Inventory.Items[i]);

                    Dictionary.Add(Inventory.Items[i].Template, data);
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

        public ItemUpgradeData(PlayerInventoryCore.ItemData item)
        {
            list = new List<ElementData>();

            Load(item.Instance);

            CheckDefaults(item.Template.Upgrades.Applicable);
        }
    }
}