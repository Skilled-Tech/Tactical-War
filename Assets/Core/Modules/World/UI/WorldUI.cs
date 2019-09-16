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
    public class WorldUI : UIElement
    {
        public WorldMapUI Map { get; protected set; }
        public RegionUI Region { get; protected set; }

        public class Element : UIElement, IModule<WorldUI>
        {
            public WorldUI World { get; protected set; }

            public Core Core { get { return Core.Instance; } }
            public ScenesCore Scenes { get { return Core.Scenes; } }

            public virtual void Configure(WorldUI data)
            {
                this.World = data;
            }

            public virtual void Init()
            {

            }
        }

        public Core Core { get { return Core.Instance; } }
        public ScenesCore Scenes { get { return Core.Scenes; } }
        public WorldCore World { get { return Core.World; } }

        protected virtual void Awake()
        {
            Map = Dependancy.Get<WorldMapUI>(gameObject);
            Region = Dependancy.Get<RegionUI>(gameObject);

            Modules.Configure(this);
        }

        protected virtual void Start()
        {
            Modules.Init(this);

            Map.OnSelect += RegionSelected;
        }

        void RegionSelected(WorldMapUI.RegionData selection)
        {
            Map.Hide();

            Region.Set(selection.Core);
        }
    }
}