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
    [CreateAssetMenu(menuName = MenuPath + "Level")]
	public class LevelCore : RegionCore.Module
	{
        [SerializeField]
        protected Sprite icon;
        public Sprite Icon { get { return icon; } }

        [SerializeField]
        protected GameScene scene;
        public GameScene Scene { get { return scene; } }

        [SerializeField]
        protected LevelBackgroundData background;
        public LevelBackgroundData Background { get { return background; } }

        public RegionCore Region { get; protected set; }
        public void Set(RegionCore region)
        {
            this.Region = region;

            this.Index = region.IndexOf(this);
        }

        public int Index { get; protected set; }
    }
}