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
    [CreateAssetMenu(menuName = MenuPath + "Element")]
    public class RegionElement : RegionsCore.Element
    {
        public const string MenuPath = Core.MenuPath + "Regions/";

        [SerializeField]
        protected Sprite icon;
        public Sprite Icon { get { return icon; } }

        #region Levels
        [SerializeField]
        protected RegionLevelElement[] levels;
        public RegionLevelElement[] Levels { get { return levels; } }

        public int Size { get { return levels.Length; } }

        public RegionLevelElement this[int index] { get { return levels[index]; } }
        #endregion

        public class Module : RegionsCore.Element
        {
            public const string MenuPath = RegionElement.MenuPath + "Modules/";
        }

        public override void Configure()
        {
            base.Configure();

            for (int i = 0; i < levels.Length; i++)
            {
                Register(levels[i]);

                levels[i].Set(this);
            }
        }
    }
}