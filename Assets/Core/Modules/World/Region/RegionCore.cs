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
    public class RegionCore : WorldCore.Module
    {
        new public const string MenuPath = WorldCore.MenuPath + "Regions/";

        public LocalizedPhraseProperty DisplayName { get; protected set; }

        [SerializeField]
        protected Sprite icon;
        public Sprite Icon { get { return icon; } }

        #region Progress
        public bool Unlocked { get; protected set; }

        public bool Finished { get { return Progress.Initial == levels.Length; } }
        #endregion

        [SerializeField]
        protected ProgressCore progress;
        public ProgressCore Progress { get { return progress; } }
        [JsonObject]
        [Serializable]
        public class ProgressCore : WorldCore.Element
        {
            [JsonProperty]
            [SerializeField]
            protected int count;
            public int Count
            {
                get
                {
                    return count;
                }
                set
                {
                    if(value > Region.Size)
                    {
                        Debug.LogWarning(Region.name + " progress value cannot be set to " + value + ", clamping to " + Region.Size);
                        value = Region.Size;
                    }

                    if(value <0)
                    {
                        Debug.LogWarning(Region.name + " progress value cannot be set to " + value + ", clamping to " + Region.Size);
                        value = 0;
                    }

                    this.count = value;
                }
            }

            public int Initial { get { return At(World.Difficulty.First); } }

            [JsonProperty]
            [JsonConverter(typeof(RegionDifficulty.Converter))]
            [SerializeField]
            protected RegionDifficulty difficulty;
            public RegionDifficulty Difficulty
            {
                get
                {
                    return difficulty;
                }
                set
                {
                    if(value == null)
                    {

                    }
                    else
                    {
                        if (World.Difficulty.Contains(value) == false)
                        {
                            Debug.LogError("difficulty " + value?.name + " not defined within the world core");
                            return;
                        }
                    }

                    this.difficulty = value;
                }
            }

            public RegionCore Region { get; protected set; }
            public virtual void Set(RegionCore region)
            {
                this.Region = region;
            }

            public virtual int At(RegionDifficulty target)
            {
                if (difficulty == null) return 0;

                if (target > difficulty) //haven't reached this difficulty yet
                    return 0;
                else if (target < difficulty) //have surpassed this difficulty
                    return Region.Size;
                else //this is the current difficulty
                    return count;
            }

            public virtual void Clear()
            {
                Count = 0;
                Difficulty = null;
            }
        }

        #region Levels
        [SerializeField]
        protected LevelCore[] levels;
        public LevelCore[] Levels { get { return levels; } }

        public int Size { get { return levels.Length; } }

        public LevelCore this[int index] { get { return levels[index]; } }

        public virtual bool Contains(LevelCore level)
        {
            return levels.Contains(level);
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

        public class Module : WorldCore.Module
        {
            new public const string MenuPath = RegionCore.MenuPath + "Modules/";
        }

        public override void Configure()
        {
            base.Configure();

            Index = World.IndexOf(this);

            progress.Set(this);
            Register(progress);

            for (int i = 0; i < levels.Length; i++)
            {
                Register(levels[i]);

                levels[i].Set(this);

                levels[i].OnComplete += LevelCompleteCallback;
            }
        }

        public override void Init()
        {
            base.Init();

            DisplayName = LocalizedPhraseProperty.Create(base.name);
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
            progress.Difficulty = World.Difficulty.First;

            Unlock(levels[0]);
        }
        public virtual void Unlock(LevelCore level)
        {
            Unlocked = true;

            progress.Count = level.Index;
        }

        public virtual void Lock()
        {
            Unlocked = false;
            progress.Clear();
        }

        public virtual void Load(LevelCore level)
        {
            Load(level, progress.Difficulty);
        }
        public virtual void Load(LevelCore level, RegionDifficulty difficulty)
        {
            if (Contains(level) == false)
                throw new ArgumentException("Trying to load " + level.name + " But it's not a part of the " + name + " Region");

            World.Load(this, level, difficulty);
        }

        void LevelCompleteCallback(LevelCore level)
        {
            if (level.IsLast)
                Complete();
        }

        public event Action OnShowStory;
        public virtual void ShowStory()
        {
            OnShowStory?.Invoke();
        }

        public delegate void CompleteDelegate(RegionCore region);
        public event CompleteDelegate OnComplete;
        protected virtual void Complete()
        {
            if (OnComplete != null) OnComplete(this);
        }
    }
}