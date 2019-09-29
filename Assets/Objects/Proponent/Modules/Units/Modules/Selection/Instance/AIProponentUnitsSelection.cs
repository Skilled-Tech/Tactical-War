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
    public class AIProponentUnitsSelection : ProponentUnitsSelection
    {
        [SerializeField]
        protected Element[] list;
        public Element[] List { get { return list; } }

        [Serializable]
        public class Element : IUnitSelectionData
        {
            [SerializeField]
            protected UnitTemplate template;
            public UnitTemplate Template { get { return template; } }

            [SerializeField]
            protected ItemUpgradesData upgrade;
            public ItemUpgradesData Upgrade { get { return upgrade; } }
        }

        public override IUnitSelectionData this[int index] => list[index];

        public override int Count => list.Length;
    }
}