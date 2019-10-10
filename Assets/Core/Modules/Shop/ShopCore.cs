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
    [Serializable]
    public class ShopCore : Core.Property
    {
        public const string MenuPath = Core.Module.MenuPath + "Shop/";

        [SerializeField]
        protected ShopSectionCore[] sections;
        public ShopSectionCore[] Sections { get { return sections; } }

        [Serializable]
        public class Element : Core.Property
        {
            public ShopCore Shop { get { return Core.Shop; } }

            public PlayFabCore PlayFab { get { return Core.PlayFab; } }
        }
        public class Module : Core.Module
        {
            new public const string MenuPath = ShopCore.MenuPath + "Modules/";

            public ShopCore Shop { get { return Core.Shop; } }

            public PlayFabCore PlayFab { get { return Core.PlayFab; } }
        }

        public PlayFabCore PlayFab { get { return Core.PlayFab; } }

        public override void Configure()
        {
            base.Configure();

            for (int i = 0; i < Sections.Length; i++)
            {
                Register(sections[i]);
            }
        }
    }
}