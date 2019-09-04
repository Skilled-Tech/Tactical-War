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
	public class PlayFabCore : Core.Module
	{
        new public const string MenuPath = Core.Module.MenuPath + "PlayFab/";

        [SerializeField]
        protected PlayFabLoginCore login;
        public PlayFabLoginCore Login { get { return login; } }

        [SerializeField]
        protected PlayFabRequestsCore requests;
        public PlayFabRequestsCore Requests { get { return requests; } }

        [SerializeField]
        protected PlayFabCatalogsCore catalogs;
        public PlayFabCatalogsCore Catalogs { get { return catalogs; } }

        [SerializeField]
        protected PlayFabInventoryCore inventory;
        public PlayFabInventoryCore Inventory { get { return inventory; } }

        public class Module : Core.Module
        {
            public PlayFabCore PlayFab { get { return Core.PlayFab; } }

            new public const string MenuPath = PlayFabCore.MenuPath + "Modules/";
        }
        
        public override void Configure()
        {
            base.Configure();

            login.Configure();
            requests.Configure();
            catalogs.Configure();
            inventory.Configure();
        }

        public override void Init()
        {
            base.Init();

            catalogs.Configure();
            catalogs.Init();
            requests.Init();
            inventory.Init();
        }
    }
}