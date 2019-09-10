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

        public const string Name = "Name";

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
            [SerializeField]
            protected ItemUpgradeType type;
            public ItemUpgradeType Type { get { return type; } }

            [SerializeField]
            protected RankData[] ranks;
            public RankData[] Ranks { get { return ranks; } }
            [Serializable]
            public class RankData
            {
                [SerializeField]
                Currency cost;
                public Currency Cost { get { return cost; } }

                [SerializeField]
                float percentage;
                public float Percentage { get { return percentage; } }

                public virtual void Load(JToken token)
                {
                    cost = new Currency(0, token[nameof(Cost)].ToObject<int>());

                    percentage = token[nameof(Percentage)].ToObject<float>();
                }

                public RankData(Currency cost, float percentage)
                {
                    this.cost = cost;

                    this.percentage = percentage;
                }
                public RankData(JToken token)
                {
                    Load(token);
                }
            }

            public ElementData(int count, Currency initalCost, float initialPercentage)
            {
                ranks = new RankData[count];

                for (int i = 0; i < ranks.Length; i++)
                    ranks[i] = new RankData(initalCost * (i + 1), initialPercentage * (i + 1));
            }

            public ElementData(JToken token)
            {
                type = Core.Items.Upgrades.Types.Find(token[nameof(Type)].ToObject<string>());

                var jArray = token[nameof(Ranks)] as JArray;

                ranks = new RankData[jArray.Count];

                for (int i = 0; i < jArray.Count; i++)
                    ranks[i] = new RankData(jArray[i]);
            }
        }

        public virtual ElementData Find(ItemUpgradeType type)
        {
            for (int i = 0; i < elements.Length; i++)
                if (elements[i].Type == type)
                    return elements[i];

            return null;
        }

        public virtual void Load(JToken token)
        {
            name = token[Name].ToObject<string>();

            var jArray = token[nameof(Elements)] as JArray;
            elements = new ElementData[jArray.Count];
            
            for (int i = 0; i < jArray.Count; i++)
            {
                elements[i] = new ElementData(jArray[i]);
            }
        }
    }
}