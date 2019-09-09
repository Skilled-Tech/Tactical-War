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

        public UnitUpgradeData this[UnitTemplate template] { get { return Dictionary[template]; } }

        public override void Configure()
        {
            base.Configure();

            Dictionary = new Dictionary<UnitTemplate, UnitUpgradeData>();

            Player.Inventory.OnRetrieved += OnInventoryRetrieved;
        }

        void OnInventoryRetrieved(PlayerInventoryCore inventory)
        {
            foreach (var item in inventory.Items)
            {
                var element = new UnitUpgradeData();

                element.Load(item);

                List.Add(element);
            }
        }
    }

    [Serializable]
    public class UnitUpgradeData
    {
        public static Core Core { get { return Core.Instance; } }

        [SerializeField]
        protected List<ElementData> list;
        public List<ElementData> List { get { return list; } }
        [Serializable]
        public class ElementData
        {
            [SerializeField]
            protected UnitUpgradeType type;
            public UnitUpgradeType Type { get { return type; } }

            [SerializeField]
            protected int value;
            public int Value { get { return value; } }

            public ElementData(JProperty property)
            {
                type = Core.Units.Upgrades.Types.Find(property.Name);

                value = property.Value.ToObject<int>();
            }
        }

        public virtual ElementData Find(UnitUpgradeType type)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].Type == type)
                    return list[i];

            return null;
        }

        public void Load(JObject jObject)
        {
            list.Clear();

            foreach (var property in jObject.Properties())
            {
                var element = new ElementData(property);

                list.Add(element);
            }
        }
        public void Load(ItemInstance item)
        {
            if (item.CustomData == null)
            {
                list.Clear();
            }
            else
            {
                if(item.CustomData.ContainsKey(UnitsUpgradesCore.Key))
                {
                    list.Clear();
                }
                else
                {
                    Load(JObject.Parse(item.CustomData[UnitsUpgradesCore.Key]));
                }
            }
        }

        public UnitUpgradeData()
        {
            list = new List<ElementData>();
        }
    }
}