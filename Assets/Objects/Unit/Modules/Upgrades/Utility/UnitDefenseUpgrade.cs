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
    public class UnitDefenseUpgrade : Unit.Module, EntityDefense.IModifier
    {
        public ItemUpgradeType Type { get { return Core.Items.Upgrades.Types.Common.Defense; } }

        public float Value
        {
            get
            {
                Upgrades.GetElements(Type, out var template, out var data);

                if (template == null || data == null) return 0f;

                return template.Ranks[data.Value].Percentage;
            }
        }
    }
}