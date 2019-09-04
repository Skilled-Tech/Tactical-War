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
    [CreateAssetMenu(menuName = MenuPath + "Assets")]
	public class PlayerUnitsCore : PlayerCore.Module
	{
        new public const string MenuPath = PlayerCore.Module.MenuPath + "Units/";

        [SerializeField]
        protected PlayerUnitsUpgradesCore upgrades;
        public PlayerUnitsUpgradesCore Upgrades { get { return upgrades; } }

        public Dictionary<UnitTemplate, UnitData> Dictionary { get; protected set; }

        [SerializeField]
        protected PlayerUnitsSelectionCore selection;
        public PlayerUnitsSelectionCore Selection { get { return selection; } }

        public class Module : PlayerCore.Module
        {
            new public const string MenuPath = PlayerUnitsCore.MenuPath + "Modules/";

            public PlayerUnitsCore Units { get { return Player.Units; } }
        }

        public override void Configure()
        {
            base.Configure();

            Dictionary = new Dictionary<UnitTemplate, UnitData>();

            for (int i = 0; i < Core.Units.Count; i++)
            {
                var data = new UnitData(Core.Units[i]);

                data = LoadData(data, i);

                data.OnChange += ()=> OnDataChanged(data);

                Dictionary.Add(Core.Units[i], data);
            }

            upgrades.Configure();
            selection.Configure();
        }

        void OnDataChanged(UnitData data)
        {
            SaveData(data.Unit);
        }

        public override void Init()
        {
            base.Init();

            upgrades.Init();
            selection.Init();
        }

        public virtual void SaveData(UnitTemplate unit)
        {
            var data = Dictionary[unit];

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            Data.Save(FormatSavePath(unit), json);
        }
        public virtual UnitData LoadData(UnitData data, int index)
        {
            var path = FormatSavePath(data.Unit);

            if(Data.Exists(path))
            {
                var json = Data.LoadText(path);

                data.Load(JObject.Parse(json));
            }
            else
            {
                if (index < 2)
                    data.Unlocked = true;
            }

            return data;
        }

        public virtual string FormatSavePath(UnitTemplate unit)
        {
            return "Player/Units/" + unit.name + ".json";
        }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class UnitData
    {
        public UnitTemplate Unit { get; protected set; }

        [JsonProperty]
        protected bool unlocked;
        public bool Unlocked
        {
            get
            {
                return unlocked;
            }
            set
            {
                unlocked = value;

                InvokeChange();
            }
        }

        [JsonProperty]
        protected UpgradesData upgrades;
        public UpgradesData Upgrades { get { return upgrades; } }
        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class UpgradesData
        {
            [HideInInspector]
            protected UnitTemplate Unit { get; set; }

            public UnitUpgradesTemplate Template { get { return Unit.Upgrades.Template; } }

            [JsonProperty]
            protected List<TypeData> types;
            public List<TypeData> Types { get { return types; } }
            [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
            public class TypeData
            {
                protected UpgradesData Parent { get; set; }

                public UnitTemplate Unit { get { return Parent.Unit; } }

                public int Index { get; protected set; }

                public UnitUpgradesTemplate.TypeData Template { get { return Unit.Upgrades.Template.Types[Index]; } }

                [JsonProperty(Order = 1)]
                public string Name { get { return Template.Target.name; } }

                [JsonProperty(Order = 2)]
                protected int value;
                public int Value
                {
                    get
                    {
                        return value;
                    }
                    protected set
                    {
                        if (value < 0) value = 0;

                        this.value = value;

                        InvokeChange();
                    }
                }

                public event Action OnChanged;
                protected virtual void InvokeChange()
                {
                    if (OnChanged != null) OnChanged();
                }

                public bool Maxed
                {
                    get
                    {
                        return Value >= Template.Ranks.Length;
                    }
                }

                public UnitUpgradesTemplate.TypeData.RankData Current
                {
                    get
                    {
                        if (Value == 0) return null;

                        return Template.Ranks[Value - 1];
                    }
                }

                public float Multiplier
                {
                    get
                    {
                        if (Current == null) return 1f;

                        return Current.Multiplier;
                    }
                }
                public float Percentage
                {
                    get
                    {
                        if (Current == null) return 0f;

                        return Current.Percentage;
                    }
                }

                public UnitUpgradesTemplate.TypeData.RankData Next
                {
                    get
                    {
                        if (Value > Template.Ranks.Length) return null;

                        return Template.Ranks[Value];
                    }
                }

                public virtual bool CanUpgrade(Funds funds)
                {
                    if (Maxed) return false;

                    if (funds.CanAfford(Next.Cost) == false) return false;

                    return true;
                }

                public virtual void Upgrade(Funds funds)
                {
                    if (CanUpgrade(funds) == false)
                        throw new Exception("Can't upgrade " + Template.Target.name);

                    funds.Take(Next.Cost);

                    Value++;
                }

                public virtual void Load(JToken token)
                {
                    Value = token[nameof(value)].ToObject<int>();
                }

                public TypeData(UpgradesData upgrade, int index)
                {
                    this.Parent = upgrade;

                    this.Index = index;

                    Value = 0;
                }
            }

            public virtual void Load(JToken token)
            {
                for (int i = 0; i < types.Count; i++)
                    types[i].Load(token[nameof(types)][i]);
            }

            public UpgradesData(UnitTemplate unit)
            {
                this.Unit = unit;

                types = new List<TypeData>();

                for (int i = 0; i < Template.Types.Length; i++)
                {
                    if(Unit.Upgrades.isApplicable(Template.Types[i].Target))
                    {
                        var instance = new TypeData(this, i);

                        instance.OnChanged += InvokeChange;

                        types.Add(instance);
                    }
                }
            }

            public event Action OnChange;
            protected virtual void InvokeChange()
            {
                if (OnChange != null) OnChange();
            }
        }

        public event Action OnChange;
        protected virtual void InvokeChange()
        {
            if (OnChange != null) OnChange();
        }

        public virtual void Load(JToken token)
        {
            unlocked = token[nameof(unlocked)].ToObject<bool>();

            upgrades.Load(token[nameof(upgrades)]);
        }

        public UnitData(UnitTemplate unit)
        {
            this.Unit = unit;

            unlocked = false;

            upgrades = new UpgradesData(unit);

            upgrades.OnChange += InvokeChange;
        }
    }
}