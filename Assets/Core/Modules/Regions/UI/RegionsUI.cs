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
    public class RegionsUI : UIElement
    {
        public Core Core { get { return Core.Instance; } }
        public ScenesCore Scenes { get { return Core.Scenes; } }
        public RegionsCore Regions { get { return Core.Regions; } }

        public class Module : UIElement, IModule<RegionsUI>
        {
            public RegionsUI Regions { get; protected set; }

            public Core Core { get { return Core.Instance; } }
            public ScenesCore Scenes { get { return Core.Scenes; } }

            public virtual void Configure(RegionsUI data)
            {
                this.Regions = data;
            }

            public virtual void Init()
            {

            }
        }
    }
}