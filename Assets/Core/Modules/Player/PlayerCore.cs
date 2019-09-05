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

using PlayFab;
using PlayFab.ClientModels;

namespace Game
{
    [CreateAssetMenu(menuName = MenuPath + "Asset")]
	public class PlayerCore : Core.Module
	{
        new public const string MenuPath = Core.Module.MenuPath + "Player/";

		[SerializeField]
        protected Funds funds = new Funds(999999);
        public Funds Funds { get { return funds; } }

        [SerializeField]
        protected PlayerUnitsCore units;
        public PlayerUnitsCore Units { get { return units; } }

        public class Module : Core.Module
        {
            new public const string MenuPath = PlayerCore.MenuPath + "Modules/";

            public PlayerCore Player { get { return Core.Player; } }
        }

        public override void Configure()
        {
            base.Configure();

            Funds.Configure(999999);

            Core.PlayFab.Inventory.OnRetrieved += OnInventoryRetrieved;

            units.Configure();
        }

        void OnInventoryRetrieved(PlayFabCore.InventoryCore result)
        {
            funds.Load(result.Currencies);
        }

        public override void Init()
        {
            base.Init();

            units.Init();
        }
    }
}