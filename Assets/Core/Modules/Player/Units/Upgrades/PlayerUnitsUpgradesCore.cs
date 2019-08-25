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
    [CreateAssetMenu(menuName = MenuPath + "Upgrades")]
	public class PlayerUnitsUpgradesCore : PlayerUnitsCore.Module
	{
        public UnitUpgradesController[] List { get; protected set; }

		public virtual UnitUpgradesController Find(UnitData unit)
        {
            for (int i = 0; i < List.Length; i++)
                if (List[i].Unit == unit)
                    return List[i];

            return null;
        }

        public override void Configure()
        {
            base.Configure();

            List = new UnitUpgradesController[Units.List.Count];

            for (int i = 0; i < Units.List.Count; i++)
                List[i] = InitController(Units.List[i]);
        }

        protected virtual UnitUpgradesController InitController(UnitData unit)
        {
            var controller = new UnitUpgradesController(unit);

            controller.OnUpgrade += OnUpgrade;

            var dataPath = FormatFilePath(unit);

            if(Data.Exists(dataPath))
            {
                var json = Data.LoadText(dataPath);

                var jObject = JObject.Parse(json);

                controller.Load(jObject);
            }

            return controller;
        }

        protected virtual void OnUpgrade(UnitUpgradesController upgrade, UnitUpgradesController.TypeController type)
        {
            var json = JsonConvert.SerializeObject(upgrade, Formatting.Indented);

            Data.Save(FormatFilePath(upgrade.Unit), json);
        }

        public static string FormatFilePath(UnitData unit)
        {
            return "Upgrades/" + unit.name + ".json";
        }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class UnitUpgradesController
    {
        public UnitData Unit { get; protected set; }

        [JsonProperty]
        public string name { get { return Unit.name; } }

        [JsonProperty]
        public List<TypeController> Types { get; protected set; }
        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class TypeController
        {
            UnitUpgradesController upgrades;

            public UnitData Unit { get { return upgrades.Unit; } }

            public UnitUpgradesData.TypeData Data { get; protected set; }

            public UnitUpgradeType Target { get { return Data.Target; } }

            [JsonProperty]
            public string name { get { return Target.name; } }

            [JsonProperty]
            public int Index { get; protected set; }

            public bool Maxed
            {
                get
                {
                    return Index >= Data.Ranks.Length;
                }
            }

            public UnitUpgradesData.TypeData.RankData Current
            {
                get
                {
                    if (Index == 0)
                        throw new NullReferenceException();

                    return Data.Ranks[Index - 1];
                }
            }

            public float Multiplier
            {
                get
                {
                    if (Index == 0) return 1f;

                    return Current.Multiplier;
                }
            }

            public float Percentage
            {
                get
                {
                    if (Index == 0) return 0f;

                    return Current.Percentage;
                }
            }

            public UnitUpgradesData.TypeData.RankData Next
            {
                get
                {
                    if (Index > Data.Ranks.Length)
                        throw new NullReferenceException();

                    return Data.Ranks[Index];
                }
            }

            public virtual bool CanUpgrade(Funds funds)
            {
                if (Maxed) return false;

                if (funds.CanAfford(Next.Cost) == false) return false;

                return true;
            }

            public event Action OnUpgrade;
            public virtual void Upgrade(Funds funds)
            {
                if (CanUpgrade(funds) == false)
                    throw new Exception("Can't upgrade " + Unit.name + "'s " + Data.Target.name);

                funds.Take(Next.Cost);

                Index++;

                if (OnUpgrade != null) OnUpgrade();

                upgrades.InvokeUpgrade(this);
            }

            public virtual void Load(JToken jToken)
            {
                Index = (int)jToken[nameof(Index)];
            }

            public TypeController(UnitUpgradesController controller, UnitUpgradesData.TypeData data)
            {
                this.upgrades = controller;

                this.Data = data;

                Index = 0;
            }
        }

        public delegate void UpgradeDelegate(UnitUpgradesController upgrades, TypeController type);
        public event UpgradeDelegate OnUpgrade;
        protected virtual void InvokeUpgrade(TypeController type)
        {
            if (OnUpgrade != null) OnUpgrade(this, type);
        }

        public virtual void Load(JObject jObject)
        {
            for (int i = 0; i < Types.Count; i++)
                Types[i].Load(jObject[nameof(Types)][i]);
        }

        public UnitUpgradesController(UnitData unit)
        {
            this.Unit = unit;

            var template = Unit.Upgrades.Template.Data;

            Types = new List<TypeController>();

            for (int i = 0; i < template.Types.Length; i++)
            {
                if(Unit.Upgrades.isApplicable(template.Types[i].Target))
                {
                    var instance = new TypeController(this, template.Types[i]);

                    Types.Add(instance);
                }
            }
        }
    }
}