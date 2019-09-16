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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Game
{
    [Serializable]
    public class WorldCore : Core.Module
	{
        #region Regions
        [SerializeField]
        protected RegionCore[] regions;
        public RegionCore[] Regions { get { return regions; } }

        public int Size { get { return regions.Length; } }

        public RegionCore this[int index] { get { return regions[index]; } }
        #endregion

        public ScenesCore Scenes { get { return Core.Scenes; } }

        public override void Configure()
        {
            base.Configure();

            for (int i = 0; i < regions.Length; i++)
            {
                Register(regions[i]);
            }
        }

        [Serializable]
        public class Module : Core.Module
        {

        }

        public class Element : ScriptableObject, Core.IModule
        {
            [SerializeField]
            protected bool unlocked;
            public bool Unlocked
            {
                get
                {
                    return unlocked;
                }
                set
                {
                    unlocked = value;
                }
            }

            public Core Core { get { return Core.Instance; } }

            public WorldCore Regions { get { return Core.World; } }

            public ScenesCore Scenes { get { return Core.Scenes; } }

            public virtual void Configure()
            {
                
            }

            public virtual void Register(Core.IModule module)
            {
                module.Configure();

                OnInit += module.Init;
            }

            public event Action OnInit;
            public virtual void Init()
            {
                if (OnInit != null) OnInit();
            }
        }
    }
}