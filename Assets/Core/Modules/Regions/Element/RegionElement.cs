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
        protected RegionLevelElement level;
        public RegionLevelElement Level { get { return level; } }

        public class Module : RegionsCore.Element
        {
            public const string MenuPath = RegionElement.MenuPath + "Modules/";
        }
    }
}