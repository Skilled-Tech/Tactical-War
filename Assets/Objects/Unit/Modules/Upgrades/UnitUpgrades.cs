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

            public virtual float Percentage
            {
                get
                {
                    Upgrades.GetElements(Type, out var template, out var data);

                    return template.Percentage.Calculate(data.Value);
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
        public virtual ItemUpgradesData.Element GetDataElement(ItemUpgradeType target)
        {
            if (Data == null) return null;

            var element = Data.Find(target);

            return element;
        }
        public virtual void GetElements(ItemUpgradeType target, out ItemUpgradesTemplate.ElementData template, out ItemUpgradesData.Element data)
        {
            template = GetTemplateElement(target);

            data = GetDataElement(target);
        }

        public virtual void Set(ItemUpgradesData data)
        {
            this.Data = data;
        }
    }
}