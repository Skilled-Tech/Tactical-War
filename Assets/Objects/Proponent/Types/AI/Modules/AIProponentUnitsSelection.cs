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
        public Element[] List => Level.Data.Level.AI.Units.List;
        [Serializable]
        public class Element : IUnitSelectionData
        {
            [SerializeField]
            protected UnitTemplate template;
            public UnitTemplate Template { get { return template; } }

            [SerializeField]
            protected int rank;
            public int Rank { get { return rank; } }

            public ItemUpgradesData Upgrades { get; protected set; }

            public virtual void Init()
            {
                Upgrades = new ItemUpgradesData(template, rank);
            }
        }

        public override IUnitSelectionData this[int index] => List[index];

        public override int Count => List.Length;
    }
}