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
        [SerializeField]
        protected RegionData[] regions;
        public RegionData[] Regions { get { return regions; } }
        [Serializable]
        public struct RegionData
        {
            [SerializeField]
#pragma warning disable CS0649
            RegionCore element;

            public RegionCore Element { get { return element; } }

            [SerializeField]
            private RegionUITemplate _UITemplate;
            public RegionUITemplate UITemplate { get { return _UITemplate; } }
#pragma warning restore CS0649

            public void Set()
            {
                _UITemplate.Set(element);
            }
        }

        public Core Core { get { return Core.Instance; } }
        public ScenesCore Scenes { get { return Core.Scenes; } }
        public WorldCore World { get { return Core.World; } }

        public class Module : UIElement, IModule<WorldUI>
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
    }
}