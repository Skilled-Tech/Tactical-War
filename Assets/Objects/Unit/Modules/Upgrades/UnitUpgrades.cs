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

        public virtual ItemUpgradesTemplate.ElementData.RankData FindCurrentRank(ItemUpgradeType type)
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