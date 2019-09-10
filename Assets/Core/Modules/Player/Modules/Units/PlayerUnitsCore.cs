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

using PlayFab;
using PlayFab.ClientModels;

namespace Game
{
    [Serializable]
    public class PlayerUnitsCore : PlayerCore.Module
	{
        [SerializeField]
        protected PlayerUnitsUpgradesCore upgrades;
        public PlayerUnitsUpgradesCore Upgrades { get { return upgrades; } }

        [SerializeField]
        protected PlayerUnitsSelectionCore selection;
        public PlayerUnitsSelectionCore Selection { get { return selection; } }

        public class Module : PlayerCore.Module
        {
            public PlayerUnitsCore Units { get { return Player.Units; } }
        }

        public override void Configure()
        {
            base.Configure();

            upgrades.Configure();
            selection.Configure();

            Player.Inventory.OnRetrieved += OnInventoryRetrieved;
        }

        void OnInventoryRetrieved(PlayerInventoryCore inventory)
        {
            
        }

        public override void Init()
        {
            base.Init();

            upgrades.Init();
            selection.Init();
        }
    }
}