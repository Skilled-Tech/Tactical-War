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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Game
{
    [CreateAssetMenu(menuName = MenuPath + "Element")]
    public class RegionCore : WorldCore.Element
    {
        public const string MenuPath = Core.MenuPath + "Regions/";

        public bool Unlocked
        {
            get
            {
                for (int i = 0; i < levels.Length; i++)
                    if (levels[i].Unlocked)
                        return true;

                return false;
            }
        }
        public virtual void Unlock()
        {
            Unlock(levels[0]);
        }

        [SerializeField]
        protected int progress = 0;
        public int Progress { get { return progress; } }

        public virtual void Unlock(LevelCore level)
        {
            progress = level.Index + 1;
        }

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

        public virtual bool Contains(LevelCore level)
        {
            for (int i = 0; i < levels.Length; i++)
                if (levels[i] == level)
                    return true;

            return false;
        }

        public class Module : WorldCore.Element
        {
            public const string MenuPath = RegionCore.MenuPath + "Modules/";
        }

        public override void Configure()
        {
            base.Configure();

            Index = World.IndexOf(this);

            for (int i = 0; i < levels.Length; i++)
            {
                Register(levels[i]);

                levels[i].Set(this);
            }
        }

        #region Query
        public int Index { get; protected set; }

        public virtual bool IsFirst { get { return Index == 0; } }
        public virtual bool IsLast { get { return Index >= World.Size - 1; } }

        public virtual RegionCore Previous
        {
            get
            {
                if (IsFirst) return null;

                return World[Index - 1];
            }
        }
        public virtual RegionCore Next
        {
            get
            {
                if (IsLast) return null;

                return World[Index + 1];
            }
        }
        #endregion

        public virtual void Load(JObject jObject)
        {
            progress = jObject[nameof(progress)].ToObject<int>();
        }

        public virtual void Load(LevelCore level)
        {
            if (Contains(level) == false)
                throw new ArgumentException("Trying to load " + level.name + " But it's not a part of the " + name + " Region");

            World.Load(this, level);
        }

        public virtual void Complete()
        {
            if(IsLast)
            {

            }
            else
            {
                Next.Unlock();
            }
        }
    }
}