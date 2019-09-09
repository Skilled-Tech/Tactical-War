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

namespace Game
{
    [CreateAssetMenu(menuName = Unit.MenuPath + "Template")]
    public class UnitTemplate : ScriptableObject
    {
        public string ID { get { return name; } }

        [SerializeField]
        protected UnitType type;
        public UnitType Type { get { return type; } }

        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        [SerializeField]
        protected Sprite icon;
        public Sprite Icon { get { return icon; } }

        [SerializeField]
        protected float health = 100f;
        public float Health { get { return health; } }

        [SerializeField]
        protected AttackData attack = new AttackData(20, 1, 1);
        public AttackData Attack { get { return attack; } }
        [Serializable]
        public class AttackData
        {
            [SerializeField]
            protected float damage;
            public float Damage { get { return damage; } }

            [SerializeField]
            protected uint range;
            public uint Range { get { return range; } }

            [SerializeField]
            protected float distance;
            public float Distance { get { return distance; } }

            public AttackData(float damage, uint range, float distance)
            {
                this.damage = damage;
                this.range = range;
                this.distance = distance;
            }
        }

        [SerializeField]
        protected UpgradesData upgrades;
        public UpgradesData Upgrades { get { return upgrades; } }
        [Serializable]
        public class UpgradesData
        {
            [SerializeField]
            protected UnitUpgradesTemplate template;
            public UnitUpgradesTemplate Template { get { return template; } }

            [SerializeField]
            protected UnitUpgradeType[] applicables = new UnitUpgradeType[2];
            public UnitUpgradeType[] Applicables { get { return applicables; } }

            public const string Override = UnitsUpgradesCore.Key + "-" + "Override";

            public virtual void Load(CatalogItem item)
            {
                if (string.IsNullOrEmpty(item.CustomData))
                {

                }
                else
                {
                    var jObject = JObject.Parse(item.CustomData);

                    if(jObject[Override] == null)
                    {
                        template = Core.Units.Upgrades.Templates.Default;
                    }
                    else
                    {
                        template = Core.Units.Upgrades.Templates.Find(jObject[Override].ToObject<string>());

                        if (template == null) template = Core.Units.Upgrades.Templates.Default;
                    }
                }
            }
        }

        [SerializeField]
        protected UnlockData unlock = new UnlockData(new Currency(0, 400));
        public UnlockData Unlock { get { return unlock; } }
        [Serializable]
        public class UnlockData
        {
            [SerializeField]
            protected Currency cost;
            public Currency Cost
            {
                get
                {
                    return cost;
                }
                set
                {
                    cost = value;
                }
            }

            public virtual void Load(CatalogItem item)
            {
                cost = new Currency(item.VirtualCurrencyPrices);
            }

            public UnlockData(Currency cost)
            {
                this.cost = cost;
            }
        }

        [SerializeField]
        protected DeploymentData deployment = new DeploymentData(new Currency(100, 0), 2f);
        public DeploymentData Deployment { get { return deployment; } }
        [Serializable]
        public class DeploymentData
        {
            [SerializeField]
            protected Currency cost;
            public Currency Cost
            {
                get
                {
                    return cost;
                }
                set
                {
                    cost = value;
                }
            }

            [SerializeField]
            protected float time;
            public float Time { get { return time; } }

            public DeploymentData(Currency cost, float time)
            {
                this.cost = cost;
                this.time = time;
            }
        }

        [SerializeField]
        [TextArea]
        protected string description;
        public string Description { get { return description; } }

        public static Core Core { get { return Core.Instance; } }

        public CatalogItem CatalogItem { get; protected set; }
        public virtual void Load(PlayFabCatalogCore catalog)
        {
            CatalogItem = null;

            for (int i = 0; i < catalog.Size; i++)
            {
                if(catalog[i].ItemId == ID)
                {
                    Load(catalog[i]);
                    break;
                }
            }
        }
        public virtual void Load(CatalogItem item)
        {
            CatalogItem = item;

            unlock.Load(item);
            upgrades.Load(item);
        }
    }
}