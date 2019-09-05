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
        protected PlayFabTitleCore title;
        public PlayFabTitleCore Title { get { return title; } }

        [SerializeField]
        protected PlayFabCatalogsCore catalogs;
        public PlayFabCatalogsCore Catalogs { get { return catalogs; } }

        [SerializeField]
        protected PlayFabInventoryCore inventory;
        public PlayFabInventoryCore Inventory { get { return inventory; } }

        [SerializeField]
        protected PlayFabPurchaseCore purchase;
        public PlayFabPurchaseCore Purchase { get { return purchase; } }

        public class Module : Core.Module
        {
            public PlayFabCore PlayFab { get { return Core.PlayFab; } }

            new public const string MenuPath = PlayFabCore.MenuPath + "Modules/";
        }

        public virtual void ForAllModules(Action<Module> action)
        {
            action(login);
            action(title);
            action(catalogs);
            action(inventory);
            action(purchase);
        }
        
        public override void Configure()
        {
            base.Configure();

            ForAllModules(x => x.Configure());
        }

        public override void Init()
        {
            base.Init();

            ForAllModules(x => x.Init());
        }
    }
}