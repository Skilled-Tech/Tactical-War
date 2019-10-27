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
    [CreateAssetMenu(menuName = MenuPath + "Level")]
	public class LevelCore : RegionCore.Module
	{
        public bool Unlocked
        {
            get
            {
                return Region.Progress.Initial >= Index;
            }
        }

        public bool UnlockedOn(RegionDifficulty difficulty)
        {
            if (difficulty == null) return true;

            if(difficulty > Region.Progress.Difficulty)
            {
                return Region.Progress.At(difficulty.Previous) == Region.Size && Index == 0;
            }
            else
            {
                return Region.Progress.At(difficulty) >= Index;
            }
        }

        public virtual void Unlock()
        {
            Region.Unlock(this);
        }

        [SerializeField]
        protected Sprite icon;
        public Sprite Icon { get { return icon; } }

        [SerializeField]
        protected GameScene scene;
        public GameScene Scene { get { return scene; } }

        [SerializeField]
        protected LevelBackgroundData background;
        public LevelBackgroundData Background { get { return background; } }

        [SerializeField]
        protected PlayerData player;
        public PlayerData Player { get { return player; } }
        [Serializable]
        public class PlayerData : ProponentData
        {
            
        }

        [SerializeField]
        protected AIData _AI;
        public AIData AI { get { return _AI; } }
        [Serializable]
        public class AIData : ProponentData
        {
            [SerializeField]
            protected UnitsData units;
            public UnitsData Units { get { return units; } }
            [Serializable]
            public class UnitsData
            {
                [SerializeField]
                protected AIProponentUnitsSelection.Element[] list;
                public AIProponentUnitsSelection.Element[] List { get { return list; } }
            }

            [SerializeField]
            protected AbilitiesData abilities;
            public AbilitiesData Abilities { get { return abilities; } }
            [Serializable]
            public class AbilitiesData
            {
                [SerializeField]
                protected AbilityTemplate[] list;
                public AbilityTemplate[] List { get { return list; } }
            }
        }

        public class ProponentData
        {
            [SerializeField]
            protected BaseData _base;
            public BaseData Base { get { return _base; } }
            [Serializable]
            public class BaseData
            {
                [SerializeField]
                protected BaseGraphicsData graphic;
                public BaseGraphicsData Graphic { get { return graphic; } }
            }
        }

        public RegionCore Region { get; protected set; }
        public void Set(RegionCore region)
        {
            this.Region = region;

            this.Index = region.IndexOf(this);
        }

        public Level Instance { get { return Level.Instance; } }

        public int Index { get; protected set; }

        public virtual bool IsFirst { get { return Index == 0; } }
        public virtual bool IsLast { get { return Index >= Region.Size - 1; } }

        public virtual LevelCore Previous
        {
            get
            {
                if (IsFirst) return null;

                return Region[Index - 1];
            }
        }
        public virtual LevelCore Next
        {
            get
            {
                if (IsLast) return null;

                return Region[Index + 1];
            }
        }

        public delegate void CompleteDelegate(LevelCore level);
        public event CompleteDelegate OnComplete;
        public virtual void Complete()
        {
            if (OnComplete != null) OnComplete(this);
        }

        public virtual void Load(RegionDifficulty difficulty)
        {
            Region.Load(this, difficulty);
        }
        public virtual void Load()
        {
            Load(Region.Progress.Difficulty);
        }
        public virtual void Reload()
        {
            Load(World.Current.Difficulty);
        }
    }
}