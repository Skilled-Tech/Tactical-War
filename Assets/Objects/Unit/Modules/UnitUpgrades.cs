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

        public UnitUpgradeData Data { get; protected set; }

        public float Damage
        {
            get
            {
                return GetCurrentPercentage(Core.Items.Upgrades.Types.Find(nameof(Damage)));
            }
        }
        public float Defense
        {
            get
            {
                return GetCurrentPercentage(Core.Items.Upgrades.Types.Find(nameof(Defense)));
            }
        }
        public float Range
        {
            get
            {
                return GetCurrentPercentage(Core.Items.Upgrades.Types.Find(nameof(Range)));
            }
        }

        public virtual ItemUpgradesTemplate.ElementData.RankData FindCurrentRank(ItemUpgradeType type)
        {
            if (Data == null) return null;


            var template = Template.Upgrades.Template.Find(type);

            if (template == null) return null;


            var data = Data.Find(type);

            if (data == null) return null;

            if (data.Value == 0 || data.Value > template.List.Length) return null;


            return template.List[data.Value - 1];
        }
        public virtual float GetCurrentPercentage(ItemUpgradeType type)
        {
            var rank = FindCurrentRank(type);

            if (rank == null) return 0f;

            return rank.Percentage;
        }

        public virtual void Set(UnitUpgradeData data)
        {
            this.Data = data;
        }
    }
}