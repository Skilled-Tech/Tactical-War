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
    [CreateAssetMenu(menuName = MenuPath + "Assets")]
	public class PlayerUnitsCore : PlayerCore.Module
	{
        new public const string MenuPath = PlayerCore.Module.MenuPath + "Units/";

        [SerializeField]
        protected PlayerUnitsUpgradesCore upgrades;
        public PlayerUnitsUpgradesCore Upgrades { get { return upgrades; } }

        [SerializeField]
        protected PlayerUnitsSelectionCore selection;
        public PlayerUnitsSelectionCore Selection { get { return selection; } }

        public class Module : PlayerCore.Module
        {
            new public const string MenuPath = PlayerUnitsCore.MenuPath + "Modules/";

            public PlayerUnitsCore Units { get { return Player.Units; } }
        }

        public IList<UnitData> List { get { return Core.Units.List; } }

        public override void Configure()
        {
            base.Configure();

            upgrades.Configure();
            selection.Configure();
        }

        public override void Init()
        {
            base.Init();

            upgrades.Init();
            selection.Init();
        }
    }
}