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
    public class PlayerProponentUnitsSelection : ProponentUnitsSelection
    {
        public static Core Core { get { return Core.Instance; } }

        public static PlayerCore Player { get { return Core.Player; } }

        public override int Count => List.Length;

        public override IUnitSelectionData this[int index] => List[index];

        public Element[] List { get; protected set; }
        [Serializable]
        public class Element : IUnitSelectionData
        {
            public UnitTemplate Template => Player.Units.Selection[Index];

            public ItemUpgradesData Upgrades => Player.Units.Upgrades.Find(Template);

            public int Index { get; protected set; }

            public Element(int index)
            {
                this.Index = index;
            }
        }

        public override void Configure(Proponent reference)
        {
            base.Configure(reference);

            List = new Element[Player.Units.Selection.Max];

            for (int i = 0; i < List.Length; i++)
                List[i] = new Element(i);
        }
    }
}