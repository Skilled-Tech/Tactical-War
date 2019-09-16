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
    public class RegionsCore : Core.Module
	{
        #region List
        [SerializeField]
        protected RegionElement[] list;
        public RegionElement[] List { get { return list; } }

        public int Count { get { return list.Length; } }

        public RegionElement this[int index] { get { return list[index]; } }
        #endregion

        public ScenesCore Scenes { get { return Core.Scenes; } }

        public override void Configure()
        {
            base.Configure();


        }

        [Serializable]
        public class Module : Core.Module
        {

        }

        public class Element : ScriptableObject, Core.IModule
        {
            public Core Core { get { return Core.Instance; } }

            public RegionsCore Regions { get { return Core.Regions; } }

            public ScenesCore Scenes { get { return Core.Scenes; } }

            public virtual void Configure()
            {
                
            }

            public virtual void Init()
            {
                
            }
        }
    }
}