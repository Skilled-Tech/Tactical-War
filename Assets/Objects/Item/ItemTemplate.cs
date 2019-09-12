﻿using System;
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Game
{
    [CreateAssetMenu(menuName = MenuPath + "Template")]
    public class ItemTemplate : ScriptableObject
    {
        public const string MenuPath = "Item/";

        public string ID { get { return base.name; } }

        [SerializeField]
        protected Sprite icon;
        public Sprite Icon { get { return icon; } }

        [SerializeField]
        [TextArea]
        protected string description;
        public string Description { get { return description; } }

        [SerializeField]
        protected Currency price = new Currency(0, 400);
        public Currency Price
        {
            get
            {
                return price;
            }
            set
            {
                price = value;
            }
        }

        [SerializeField]
        protected UpgradesData upgrades;
        public UpgradesData Upgrades { get { return upgrades; } }
        [Serializable]
        public class UpgradesData
        {
            public const string MenuPath = ItemTemplate.MenuPath + "Upgrades/";

            [SerializeField]
            protected ItemUpgradesTemplate template;
            public ItemUpgradesTemplate Template { get { return template; } }

            [SerializeField]
            protected ItemUpgradeType[] applicable = new ItemUpgradeType[] { };
            public ItemUpgradeType[] Applicable { get { return applicable; } }

            public virtual bool isApplicable(ItemUpgradeType type)
            {
                return applicable.Contains(type);
            }

            public const string Key = ItemsUpgradesCore.Key;

            public virtual void Load(CatalogItem item)
            {
                void LoadTemplate(JToken token)
                {
                    if (token == null)
                        template = Items.Upgrades.Templates.Default;
                    else
                    {
                        var value = token.ToObject<string>();

                        if (string.IsNullOrEmpty(value))
                            template = null;
                        else
                            template = Items.Upgrades.Templates.Find(value);
                    }
                }

                void LoadApplicable(JArray array)
                {
                    applicable = new ItemUpgradeType[array.Count];

                    for (int i = 0; i < array.Count; i++)
                        applicable[i] = Items.Upgrades.Types.Find(array[i].ToObject<string>());
                }

                if (string.IsNullOrEmpty(item.CustomData))
                {

                }
                else
                {
                    var jObject = JObject.Parse(item.CustomData);

                    if (jObject[Key] == null)
                    {

                    }
                    else
                    {
                        LoadTemplate(jObject[Key][nameof(template)]);

                        LoadApplicable(jObject[Key][nameof(applicable)] as JArray);
                    }
                }
            }
        }

        public static Core Core { get { return Core.Instance; } }
        public static ItemsCore Items { get { return Core.Items; } }

        public CatalogItem CatalogItem { get; protected set; }
        public virtual void LoadCatalog(PlayFabCatalogCore catalog)
        {
            CatalogItem = null;

            for (int i = 0; i < catalog.Size; i++)
            {
                if (catalog[i].ItemId == ID)
                {
                    Load(catalog[i]);
                    break;
                }
            }
        }
        public virtual void Load(CatalogItem item)
        {
            CatalogItem = item;

            price = new Currency(item.VirtualCurrencyPrices);

            upgrades.Load(item);
        }

        public class Converter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return typeof(ItemTemplate).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.Value == null) return null;

                var ID = reader.Value as string;

                var template = Items.Find(ID);

                return template;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var template = value as ItemTemplate;

                serializer.Serialize(writer, template.ID);
            }

            [Preserve]
            public Converter()
            {

            }
        }
    }
}