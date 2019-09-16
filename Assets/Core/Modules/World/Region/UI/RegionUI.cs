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
	public class RegionUI : WorldUI.Element
	{
        public LevelsUIList Levels { get; protected set; }

        public class Element : UIElement, IModule<RegionUI>
        {
            public RegionUI Region { get; protected set; }

            public Core Core { get { return Core.Instance; } }
            public ScenesCore Scenes { get { return Core.Scenes; } }

            public virtual void Configure(RegionUI data)
            {
                this.Region = data;
            }

            public virtual void Init()
            {

            }
        }

        public override void Configure(WorldUI data)
        {
            base.Configure(data);

            Modules.Configure(this);
        }

        public override void Init()
        {
            base.Init();

            Modules.Init(this);
        }

        public RegionCore Region { get; protected set; }

        public virtual void Set(RegionCore data)
        {
            this.Region = data;

            Levels.Set(data);
        }
    }
}