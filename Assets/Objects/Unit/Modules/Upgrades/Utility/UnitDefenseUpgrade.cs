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
        [SerializeField]
        protected ItemUpgradeType type;
        public ItemUpgradeType Type { get { return type; } }

        public float Value
        {
            get
            {
                var rank = Unit.Upgrades.FindCurrentRank(type);

                if (rank == null) return 0f;

                return rank.Percentage;
            }
        }
    }
}