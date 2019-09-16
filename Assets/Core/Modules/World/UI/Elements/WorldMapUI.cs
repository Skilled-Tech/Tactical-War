﻿using System;
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
	public class WorldMapUI : WorldUI.Element
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
    }
}