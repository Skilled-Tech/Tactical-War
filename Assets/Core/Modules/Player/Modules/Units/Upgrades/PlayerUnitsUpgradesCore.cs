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
        public Dictionary<ItemTemplate, ItemUpgradesData> Dictionary { get; protected set; }

        public ItemsCore Items { get { return Core.Items; } }

        public ItemUpgradesData Find(ItemTemplate template)
        {
            if (Dictionary.ContainsKey(template) == false) return null;

            return Dictionary[template];
        }

        public PlayerInventoryCore Inventory { get { return Player.Inventory; } }

        public override void Configure()
        {
            base.Configure();

            Dictionary = new Dictionary<ItemTemplate, ItemUpgradesData>();

            Player.Inventory.OnUpdate += OnInventoryChanged;
        }

        void OnInventoryChanged()
        {
            Dictionary.Clear();

            for (int i = 0; i < Inventory.Items.Count; i++)
            {
                if (Items.Upgrades.IsUpgradable(Inventory.Items[i].Template.CatalogItem))
                {
                    var data = new ItemUpgradesData(Inventory.Items[i]);

                    if(Dictionary.ContainsKey(Inventory.Items[i].Template))
                    {
                        Debug.LogWarning("Duplicate Unit found, duplicate: " + Inventory.Items[i].Template);
                        continue;
                    }

                    Dictionary.Add(Inventory.Items[i].Template, data);
                }
            }
        }

        public virtual ItemUpgradesTemplate.ElementData GetTemplateElement(ItemTemplate item, ItemUpgradeType type)
        {
            var element = item.Upgrades.Template.Find(type);

            return element;
        }
        public virtual ItemUpgradesData.Element GetDataElement(ItemTemplate item, ItemUpgradeType type)
        {
            var data = Find(item);

            if (data == null) return null;

            var element = data.Find(type);

            return element;
        }
        public virtual void GetElements(ItemTemplate item, ItemUpgradeType type, out ItemUpgradesTemplate.ElementData template, out ItemUpgradesData.Element data)
        {
            template = GetTemplateElement(item, type);

            data = GetDataElement(item, type);
        }
    }

    [Serializable]
    public class ItemUpgradesData
    {
        public static Core Core { get { return Core.Instance; } }
        public static ItemsCore Items { get { return Core.Items; } }

        [JsonProperty]
        [SerializeField]
        protected List<Element> list;
        public List<Element> List { get { return list; } }
        [Serializable]
        public class Element
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

            public int Index => value - 1;

            public Element()
            {

            }
            public Element(ItemUpgradeType type, int value)
            {
                this.type = type;

                this.value = value;
            }
            public Element(ItemUpgradeType type) : this(type, 0)
            {
                
            }
        }

        public virtual Element Find(ItemUpgradeType type)
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

        public virtual void CheckDefaults(ItemTemplate template)
        {
            CheckDefaults(template.Upgrades.Applicable);
        }
        public virtual void CheckDefaults(ItemUpgradeType[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                var element = Find(types[i]);

                if (element == null)
                {
                    var instance = new Element(types[i]);

                    list.Add(instance);
                }
            }
        }

        public ItemUpgradesData(PlayerInventoryCore.ItemData item)
        {
            list = new List<Element>();

            Load(item.Instance);

            CheckDefaults(item.Template);
        }
        public ItemUpgradesData(UnitTemplate template, List<Element> list)
        {
            this.list = list;

            CheckDefaults(template);
        }
        public ItemUpgradesData(ItemTemplate template, int rank)
        {
            list = new List<Element>();

            for (int i = 0; i < template.Upgrades.Applicable.Length; i++)
            {
                var instance = new Element(template.Upgrades.Applicable[i], rank);

                list.Add(instance);
            }
        }
    }
}