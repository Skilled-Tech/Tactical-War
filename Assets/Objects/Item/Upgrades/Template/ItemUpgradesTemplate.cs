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

        [SerializeField]
        protected List<ElementData> list = new List<ElementData>()
        {
            new ElementData(5, new Currency(0, 200), 10),
            new ElementData(5, new Currency(0, 200), 10),
            new ElementData(5, new Currency(0, 200), 20),
        };
        public List<ElementData> List { get { return list; } }
        [Serializable]
        public class ElementData
        {
            [SerializeField]
            protected ItemUpgradeType type;
            public ItemUpgradeType Type { get { return type; } }

            [SerializeField]
            protected RankData[] list;
            public RankData[] List { get { return list; } }
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
                list = new RankData[count];

                for (int i = 0; i < list.Length; i++)
                    list[i] = new RankData(initalCost * (i + 1), initialPercentage * (i + 1));
            }

            public ElementData(JProperty property)
            {
                type = Core.Items.Upgrades.Types.Find(property.Name);

                var jArray = JArray.FromObject(property.Value);

                list = new RankData[jArray.Count];

                for (int i = 0; i < jArray.Count; i++)
                    list[i] = new RankData(jArray[i]);
            }
        }

        public virtual ElementData Find(ItemUpgradeType type)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].Type == type)
                    return list[i];

            return null;
        }

        public virtual void Load(JProperty property)
        {
            name = property.Name;

            list.Clear();
            foreach (var item in property.Value.Children<JProperty>())
            {
                var element = new ElementData(item);

                list.Add(element);
            }
        }
    }
}