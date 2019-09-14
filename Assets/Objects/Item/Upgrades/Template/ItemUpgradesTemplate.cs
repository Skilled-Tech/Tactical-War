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

        public const string Name = "name";

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
            protected RankData[] ranks;
            public RankData[] Ranks { get { return ranks; } }
            [Serializable]
            public class RankData
            {
                [JsonProperty]
                [SerializeField]
                Currency cost;
                public Currency Cost { get { return cost; } }

                [JsonProperty]
                [SerializeField]
                float percentage;
                public float Percentage { get { return percentage; } }

                [JsonProperty]
                [SerializeField]
                protected ItemRequirementData[] requirements;
                public ItemRequirementData[] Requirements { get { return requirements; } }

                public RankData()
                {

                }
                public RankData(Currency cost, float percentage)
                {
                    this.cost = cost;

                    this.percentage = percentage;
                }
            }

            public ElementData()
            {

            }
            public ElementData(int count, Currency initalCost, float initialPercentage)
            {
                ranks = new RankData[count];

                for (int i = 0; i < ranks.Length; i++)
                    ranks[i] = new RankData(initalCost * (i + 1), initialPercentage * (i + 1));
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
}