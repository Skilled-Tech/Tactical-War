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
    [CreateAssetMenu(menuName = UnitUpgrades.MenuPath + "Data")]
    public class UnitUpgradesData : ScriptableObject
	{
        [SerializeField]
        protected Property[] list = new Property[]
        {
            new Property(10, new Currency(300, 0), 6),
            new Property(10, new Currency(300, 0), 6),
            new Property(10, new Currency(300, 0), 15)
        };
        public Property[] List { get { return list; } }

        public int Count { get { return list.Length; } }

        public Property this[int index] { get { return list[index]; } }

        [Serializable]
        public class Property
        {
            [SerializeField]
            protected UnitUpgradeType type;
            public UnitUpgradeType Type { get { return type; } }

            [SerializeField]
            protected Data[] ranks;
            public Data[] Ranks { get { return ranks; } }

            public Data this[int index] { get { return ranks[index]; } }

            [Serializable]
            public class Data
            {
                [SerializeField]
                protected Currency cost;
                public Currency Cost { get { return cost; } }

                [SerializeField]
                protected float percentage;
                public float Percentage { get { return percentage; } }

                public float Multiplier
                {
                    get
                    {
                        return 1f + (percentage / 100);
                    }
                }

                public virtual float Sample(float value)
                {
                    return value * Multiplier;
                }

                public Data(Currency cost, float percentage)
                {
                    this.cost = cost;

                    this.percentage = percentage;
                }
            }

            public int Max { get { return ranks.Length; } }

            public uint Index { get; protected set; } = 0;

            public Data Current
            {
                get
                {
                    if (Index == 0) return null;

                    return ranks[Index - 1];
                }
            }
            public bool Maxed { get { return Current == ranks.Last(); } }

            public virtual float Sample(float value)
            {
                if (Current == null) return value;

                return Current.Sample(value);
            }
            public virtual float Multiplier
            {
                get
                {
                    if (Current == null) return 1f;

                    return Current.Multiplier;
                }
            }

            public Data Next
            {
                get
                {
                    if (Maxed) return null;

                    return ranks[Index];
                }
            }
            public bool CanUpgrade(Funds funds)
            {
                if (Maxed) return false;

                return funds.CanAfford(Next.Cost);
            }

            public Property(int count, Currency cost, float percentage)
            {
                ranks = new Data[count];

                for (int i = 0; i < ranks.Length; i++)
                {
                    ranks[i] = new Data(cost * (i + 1), percentage * (i + 1));
                }
            }

            public event Action OnUpgrade;
            public virtual void Upgrade(Funds funds)
            {
                if (!CanUpgrade(funds))
                    throw new Exception("Error while trying to upgrade " + GetType().Name);

                funds.Take(Next.Cost);
                Index++;

                if (OnUpgrade != null) OnUpgrade();
            }
        }
    }
}