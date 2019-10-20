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
    public class UnitUpgrades : Unit.Module
    {
        public const string MenuPath = Unit.MenuPath + "Upgrades/";

        public ItemUpgradesData Data { get; protected set; }

        [Serializable]
        public abstract class Property
        {
            public abstract ItemUpgradeType Type { get; }

            public abstract float Base { get; }

            public virtual ItemUpgradesTemplate.ElementData.RankData Rank
            {
                get
                {
                    Upgrades.GetElements(Type, out var template, out var data);

                    if (template == null) return null;

                    if (data == null) return null;

                    return template.Ranks[data.Value];
                }
            }

            public virtual float Percentage
            {
                get
                {
                    if (Rank == null) return 0f;

                    return Rank.Percentage;
                }
            }

            public virtual float Multiplier => 1f + (Percentage / 100f);

            public float Value { get { return Base * Multiplier; } }

            public Unit Unit { get; protected set; }
            public virtual void Init(Unit unit)
            {
                this.Unit = unit;
            }

            public Proponent Leader { get { return Leader; } }
            public UnitUpgrades Upgrades { get { return Unit.Upgrades; } }
            public UnitTemplate Template { get { return Unit.Template; } }

            public static Core Core { get { return Core.Instance; } }
        }

        public virtual ItemUpgradesTemplate.ElementData GetTemplateElement(ItemUpgradeType target)
        {
            var element = Template.Upgrades.Template.Find(target);

            return element;
        }
        public virtual ItemUpgradesData.ElementData GetDataElement(ItemUpgradeType target)
        {
            var element = Data.Find(target);

            return element;
        }
        public virtual void GetElements(ItemUpgradeType target, out ItemUpgradesTemplate.ElementData template, out ItemUpgradesData.ElementData data)
        {
            template = GetTemplateElement(target);

            data = GetDataElement(target);
        }

        public virtual ItemUpgradesTemplate.ElementData.RankData DEP_FindCurrentRank(ItemUpgradeType type)
        {
            if (Data == null) return null;

            var template = GetTemplateElement(type);
            if (template == null) return null;

            var data = GetDataElement(type);
            if (data == null) return null;

            if (data.Value == 0) return null;
            if (data.Value > template.Ranks.Length) return template.Ranks.Last();

            return template.Ranks[data.Value - 1];
        }

        public virtual void Set(ItemUpgradesData data)
        {
            this.Data = data;
        }
    }
}