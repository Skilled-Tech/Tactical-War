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
    [CreateAssetMenu(menuName = MenuPath + "Element")]
    public class RegionCore : WorldCore.Element
    {
        public const string MenuPath = Core.MenuPath + "Regions/";

        [SerializeField]
        protected Sprite icon;
        public Sprite Icon { get { return icon; } }

        #region Levels
        [SerializeField]
        protected LevelCore[] levels;
        public LevelCore[] Levels { get { return levels; } }

        public int Size { get { return levels.Length; } }

        public LevelCore this[int index] { get { return levels[index]; } }

        public int IndexOf(LevelCore level)
        {
            for (int i = 0; i < levels.Length; i++)
                if (levels[i] == level)
                    return i;

            throw new ArgumentException();
        }
        #endregion

        public class Module : WorldCore.Element
        {
            public const string MenuPath = RegionCore.MenuPath + "Modules/";
        }

        public override void Configure()
        {
            base.Configure();

            for (int i = 0; i < levels.Length; i++)
            {
                Register(levels[i]);

                levels[i].Set(this);
            }
        }

        public virtual void Load(LevelCore level)
        {
            SceneManager.LoadScene(Scenes.Level, LoadSceneMode.Single);

            SceneManager.LoadScene(level.Scene.Name, LoadSceneMode.Additive);
        }
    }
}