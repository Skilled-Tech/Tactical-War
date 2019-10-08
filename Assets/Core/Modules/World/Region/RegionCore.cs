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
    [JsonObject]
    [CreateAssetMenu(menuName = MenuPath + "Element")]
    public class RegionCore : WorldCore.Element
    {
        public const string MenuPath = Core.MenuPath + "Regions/";

        [SerializeField]
        protected Sprite icon;
        public Sprite Icon { get { return icon; } }

        #region Progress
        public bool Unlocked { get; protected set; }

        [JsonProperty]
        [SerializeField]
        protected int progress = 0;
        public int Progress { get { return progress; } }

        public bool Finished { get { return progress == levels.Length; } }
        #endregion

        #region Levels
        [SerializeField]
        protected LevelCore[] levels;
        public LevelCore[] Levels { get { return levels; } }

        public int Size { get { return levels.Length; } }

        public LevelCore this[int index] { get { return levels[index]; } }

        public virtual bool Contains(LevelCore level)
        {
            for (int i = 0; i < levels.Length; i++)
                if (levels[i] == level)
                    return true;

            return false;
        }

        public int IndexOf(LevelCore level)
        {
            for (int i = 0; i < levels.Length; i++)
                if (levels[i] == level)
                    return i;

            throw new ArgumentException();
        }
        #endregion

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

        [JsonProperty]
        [JsonConverter(typeof(RegionDifficulty.Converter))]
        [SerializeField]
        protected RegionDifficulty difficulty;
        public RegionDifficulty Difficulty { get { return difficulty; } }

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

                levels[i].OnComplete += LevelCompleteCallback;
            }
        }

        public virtual void Parse(JToken jToken)
        {
            Unlocked = true;

            var json = jToken.ToString();

            JsonConvert.PopulateObject(json, this);
        }
        public virtual void ApplyDefaults()
        {
            if (Index == 0 || Previous.Finished)
                Unlock();
            else
                Lock();
        }

        public virtual void Unlock()
        {
            difficulty = World.Difficulty.List[0];

            Unlock(levels[0]);
        }
        public virtual void Unlock(LevelCore level)
        {
            Unlocked = true;

            progress = level.Index;
        }

        public virtual void Lock()
        {
            Unlocked = false;
            progress = 0;
            difficulty = null;
        }

        public virtual void Load(LevelCore level)
        {
            Load(level, difficulty);
        }
        public virtual void Load(LevelCore level, RegionDifficulty difficulty)
        {
            if (Contains(level) == false)
                throw new ArgumentException("Trying to load " + level.name + " But it's not a part of the " + name + " Region");

            World.Load(this, level);
        }

        void LevelCompleteCallback(LevelCore level)
        {
            if (level.IsLast)
                Complete();
        }

        public delegate void CompleteDelegate(RegionCore region);
        public event CompleteDelegate OnComplete;
        protected virtual void Complete()
        {
            if (OnComplete != null) OnComplete(this);
        }
    }
}