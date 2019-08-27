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

namespace Game
{
    [CreateAssetMenu(menuName = UnitUpgrades.MenuPath + "Template")]
    public class UnitUpgradesTemplate : ScriptableObject
    {
        [SerializeField]
        TypeData[] types = new TypeData[]
            {
                new TypeData(null, 10, new Currency(0, 300), 6),
                new TypeData(null, 10, new Currency(0, 300), 6),
                new TypeData(null, 10, new Currency(0, 300), 15)
            };
        public TypeData[] Types { get { return types; } }

        [Serializable]
        public class TypeData
        {
            [SerializeField]
            UnitUpgradeType target;
            public UnitUpgradeType Target { get { return target; } }

            [SerializeField]
            RankData[] ranks;
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

                public float Multiplier
                {
                    get
                    {
                        return 1f + (percentage / 100);
                    }
                }

                public float Sample(float value)
                {
                    return value * Multiplier;
                }

                public RankData(Currency cost, float percentage)
                {
                    this.cost = cost;

                    this.percentage = percentage;
                }
            }

            public TypeData(UnitUpgradeType type, RankData[] ranks)
            {
                this.target = type;
                this.ranks = ranks;
            }

            public TypeData(UnitUpgradeType type, int ranksCount, Currency initalCost, float initialPercentage)
            {
                this.target = type;

                ranks = new RankData[ranksCount];

                for (int i = 0; i < ranks.Length; i++)
                    ranks[i] = new RankData(initalCost * (i + 1), initialPercentage * (i + 1));
            }
        }
    }
}