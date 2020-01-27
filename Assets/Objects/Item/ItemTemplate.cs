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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine.Scripting;

namespace Game
{
    [CreateAssetMenu(menuName = MenuPath + "Template")]
    public partial class ItemTemplate : ScriptableObject
    {
        public const string MenuPath = Core.GameMenuPath + "Item/";

        public string ID
        {
            get
            {
                return base.name.Replace(' ', '_').ToLower();
            }
        }

        public virtual bool CompareID(string value)
        {
            return CompareID(ID, value);
        }

        public static bool CompareID(string id1, string id2)
        {
            return String.Equals(id1, id2, StringComparison.OrdinalIgnoreCase);
        }

        public LocalizedPhraseProperty DisplayName { get; protected set; }

        [SerializeField]
        protected IconProperty icon;
        public IconProperty Icon { get { return icon; } }

        public LocalizedPhraseProperty Description { get; protected set; }

        [SerializeField]
        protected List<Currency> prices;
        public List<Currency> Prices
        {
            get
            {
                return prices;
            }
            set
            {
                prices = value;
            }
        }

        public Currency Price { get { return prices[0]; } }

        [SerializeField]
        protected UpgradesData upgrades;
        public UpgradesData Upgrades { get { return upgrades; } }
        [Serializable]
        public class UpgradesData
        {
            public const string Key = ItemsUpgradesCore.Key;

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

        [SerializeField]
        protected VisibilityData visibility = new VisibilityData(true, true);
        public VisibilityData Visiblity { get { return visibility; } }
        [Serializable]
        public class VisibilityData
        {
            [SerializeField]
            protected bool inventory;
            public bool Inventory { get { return inventory; } }

            [SerializeField]
            protected bool shop;
            public bool Shop { get { return shop; } }

            [SerializeField]
            protected bool roster;
            public bool Rotster { get { return roster; } }

            public VisibilityData(bool inventory, bool shop)
            {
                this.inventory = inventory;
                this.shop = shop;
            }
        }

        [SerializeField]
        protected UnlockData unlock;
        public UnlockData Unlock { get { return unlock; } }
        [Serializable]
        public class UnlockData
        {
            [SerializeField]
            protected LevelCore level;
            public LevelCore Level { get { return level; } }

            public bool Available
            {
                get
                {
                    if (level == null) return true;

                    return level.Completed;
                }
            }
        }

        public static Core Core { get { return Core.Instance; } }
        public static ItemsCore Items { get { return Core.Items; } }

        protected virtual void Reset()
        {

        }

        public virtual void Init()
        {
            DisplayName = LocalizedPhraseProperty.Create(base.name);

            Description = LocalizedPhraseProperty.Create(base.name + " " + nameof(Description));
        }

        public CatalogItem CatalogItem { get; protected set; }
        public virtual void Load(CatalogItem item)
        {
            CatalogItem = item;

            prices.Clear();
            if (item.VirtualCurrencyPrices == null || item.VirtualCurrencyPrices.Count == 0)
            {

            }
            else
            {
                foreach (var pair in item.VirtualCurrencyPrices)
                {
                    var element = new Currency(pair);

                    prices.Add(element);
                }
            }

            upgrades.Load(item);
        }

        [Preserve]
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

                if(template == null && string.IsNullOrEmpty(ID) == false)
                {
                    Debug.LogError("JSON Convert Error, No Item Template found with ID: " + ID);
                }

                return template;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value == null)
                    serializer.Serialize(writer, null);
                else
                {
                    var template = value as ItemTemplate;

                    serializer.Serialize(writer, template.ID);
                }
            }

            public Converter()
            {

            }
        }
    }

    //TODO move to appropriate folder
    [Serializable]
    public class IconProperty
    {
        [SerializeField]
        protected Sprite sprite;
        public Sprite Sprite
        {
            get => sprite;
            set => sprite = value;
        }

        [SerializeField]
        [Range(-360f, 360f)]
        protected float tilt;
        public float Tilt { get { return tilt; } }

        [SerializeField]
        protected float scale = 1f;
        public float Scale { get { return scale; } }

        public virtual void ApplyTo(Image image)
        {
            image.sprite = sprite;

            image.rectTransform.localEulerAngles = Vector3.forward * tilt;

            image.rectTransform.localScale = Vector3.one * scale;
        }
    }
}