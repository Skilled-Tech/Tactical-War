﻿using System;
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
using Newtonsoft.Json.Linq;

using PlayFab;

namespace Game
{
    [Serializable]
	public class UnitsCore : Core.Module
	{
        #region List
        [SerializeField]
        protected UnitTemplate[] list;
        public UnitTemplate[] List { get { return list; } }

        public UnitTemplate this[int index] { get { return list[index]; } }

        public int Count { get { return list.Length; } }
        #endregion

        [SerializeField]
        protected UnitsUpgradesCore upgrades;
        public UnitsUpgradesCore Upgrades { get { return upgrades; } }

        [Serializable]
        public class Module : Core.Module
        {
            public UnitsCore Units { get { return Core.Units; } }
        }

        public override void Configure()
        {
            base.Configure();

            Core.PlayFab.Catalog.OnRetrieved += OnCatalogRetrieved;

            upgrades.Configure();
        }

        public override void Init()
        {
            base.Init();

            upgrades.Init();
        }

        void OnCatalogRetrieved(PlayFabCatalogCore catalog)
        {
            for (int i = 0; i < list.Length; i++)
            {
                list[i].Load(catalog);
            }
        }

        public virtual UnitTemplate Find(string name)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].name == name)
                    return list[i];

            return null;
        }
    }
}