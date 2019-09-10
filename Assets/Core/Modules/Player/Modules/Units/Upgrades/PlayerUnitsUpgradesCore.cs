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
        public Dictionary<UnitTemplate, UnitUpgradeData> Dictionary { get; protected set; }

        public List<UnitUpgradeData> List;

        public UnitUpgradeData Find(UnitTemplate template)
        {
            if (Dictionary.ContainsKey(template) == false) return null;

            return Dictionary[template];
        }

        public override void Configure()
        {
            base.Configure();

            Dictionary = new Dictionary<UnitTemplate, UnitUpgradeData>();

            for (int i = 0; i < Core.Items.Units.Count; i++)
            {
                var template = Core.Items.Units[i];
                var instance = Player.Inventory.Find(Core.Items.Units[i].CatalogItem);
                var data = new UnitUpgradeData(instance, template);

                Dictionary.Add(template, data);
            }

            List = Dictionary.Values.ToList();

            Player.Inventory.OnRetrieved += OnInventoryRetrieved;
        }

        void OnInventoryRetrieved(PlayerInventoryCore inventory)
        {
            foreach (var pair in Dictionary)
            {
                var itemInstance = inventory.Find(pair.Key.CatalogItem);

                pair.Value.Load(itemInstance, pair.Key);
            }

            List = Dictionary.Values.ToList();
        }
    }

    [Serializable]
    public class UnitUpgradeData
    {
        public static Core Core { get { return Core.Instance; } }
        public static ItemsCore Items { get { return Core.Items; } }

        [SerializeField]
        protected List<ElementData> list;
        public List<ElementData> List { get { return list; } }
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
                type = Items.Upgrades.Types.Find(token[nameof(Type)]);

                value = token[nameof(Value)].ToObject<int>();
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

        void Load(JArray array)
        {
            list = new List<ElementData>();

            for (int i = 0; i < array.Count; i++)
            {
                var element = new ElementData(array[i]);

                list.Add(element);
            }
        }
        void Load(string json)
        {
            Load(JArray.Parse(json));
        }
        public void Load(ItemInstance instance, ItemTemplate template)
        {
            if (instance.CustomData == null)
            {
                list.Clear();
            }
            else
            {
                if(instance.CustomData.ContainsKey(ItemsUpgradesCore.Key))
                    Load(instance.CustomData[ItemsUpgradesCore.Key]);
                else
                    list.Clear();
            }

            AddDefaults(template);
        }

        void AddDefaults(ItemTemplate template)
        {
            IList<ItemUpgradeType> applicables = template.Upgrades.Applicable;

            for (int i = 0; i < applicables.Count; i++)
            {
                if (Find(applicables[i]) == null)
                {
                    var element = new ElementData(applicables[i]);

                    list.Add(element);
                }
            }
        }

        public UnitUpgradeData()
        {
            list = new List<ElementData>();
        }
        public UnitUpgradeData(ItemInstance item, ItemTemplate template) : this()
        {
            Load(item, template);
        }
    }
}