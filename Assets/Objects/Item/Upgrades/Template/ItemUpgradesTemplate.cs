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

namespace Game
{
    [CreateAssetMenu(menuName = ItemTemplate.UpgradesData.MenuPath + "Template")]
    public class ItemUpgradesTemplate : ScriptableObject
    {
        public static Core Core { get { return Core.Instance; } }
        public static ItemsCore Items { get { return Core.Items; } }

        public const string ID = "name";

        [JsonProperty]
        [SerializeField]
        protected ElementData[] elements = new ElementData[]
        {
            new ElementData(5, new Currency(0, 200), 10),
            new ElementData(5, new Currency(0, 200), 10),
            new ElementData(5, new Currency(0, 200), 20),
        };
        public ElementData[] Elements { get { return elements; } }
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
            protected FactorialValue cost;
            public FactorialValue Cost { get { return cost; } }

            [JsonProperty]
            [SerializeField]
            protected FactorialValue percentage;
            public FactorialValue Percentage { get { return percentage; } }

            public float CalculateMultiplier(float percentage) => 1f + (percentage / 100f);

            [JsonProperty(ItemConverterType = typeof(ItemStack.TextListConverter))]
            [SerializeField]
            protected ItemStack[][] requirements;
            public ItemStack[][] Requirements { get { return requirements; } }

            public ElementData()
            {

            }
        }

        public virtual ElementData Find(ItemUpgradeType type)
        {
            for (int i = 0; i < elements.Length; i++)
                if (elements[i].Type == type)
                    return elements[i];

            return null;
        }

        public virtual void Load(string json)
        {
            JsonConvert.PopulateObject(json, this);
        }
    }

    public class FactorialValue
    {
        [JsonProperty]
        [SerializeField]
        protected float initial;
        public float Initial { get { return initial; } }

        [JsonProperty]
        [SerializeField]
        protected float multiplier;
        public float Multiplier { get { return multiplier; } }

        public virtual float Calculate(int i)
        {
            return this.initial + (this.multiplier * i);
        }

        public virtual float CalculateMultiplier(int i) => 1f + (Calculate(i) / 100f);

        public FactorialValue(float initial, float multiplier)
        {
            this.initial = initial;
            this.multiplier = multiplier;
        }
    }
}