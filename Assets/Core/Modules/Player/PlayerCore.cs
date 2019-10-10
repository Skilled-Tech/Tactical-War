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
    [Serializable]
    public class PlayerCore : Core.Property
    {
        [SerializeField]
        protected PlayerInventoryCore inventory;
        public PlayerInventoryCore Inventory { get { return inventory; } }

        public Funds Funds { get { return inventory.Funds; } }

        [SerializeField]
        protected PlayerUnitsCore units;
        public PlayerUnitsCore Units { get { return units; } }

        public class Module : Core.Property
        {
            public PlayerCore Player { get { return Core.Player; } }
        }

        public override void Configure()
        {
            base.Configure();

            Register(inventory);
            Register(units);
        }
    }
}