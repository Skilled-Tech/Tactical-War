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
        protected MusicTrack music;
        public MusicTrack Music { get { return music; } }

        [SerializeField]
        protected PlayerData player;
        public PlayerData Player { get { return player; } }
        [Serializable]
        public class PlayerData : ProponentProperty
        {
            
        }

        [SerializeField]
        protected AIData _AI;
        public AIData AI { get { return _AI; } }
        [Serializable]
        public class AIData : ProponentProperty
        {
            [SerializeField]
            protected int agression = 1; //a value to describe how many units the AI will overtake you with
            public int Agression { get { return agression; } }

            [SerializeField]
            protected UnitsProperty units;
            public UnitsProperty Units { get { return units; } }
            [Serializable]
            public class UnitsProperty : Property
            {
                [SerializeField]
                protected AIProponentUnitsSelection.Element[] list;
                public AIProponentUnitsSelection.Element[] List { get { return list; } }

                public override void Configure()
                {
                    base.Configure();

                    for (int i = 0; i < list.Length; i++)
                        list[i].Init();
                }
            }
            
            [SerializeField]
            protected AbilitiesProperty abilities;
            public AbilitiesProperty Abilities { get { return abilities; } }
            [Serializable]
            public class AbilitiesProperty : Property
            {
                [SerializeField]
                protected AbilityTemplate[] list;
                public AbilityTemplate[] List { get { return list; } }
            }

            public override void Configure()
            {
                base.Configure();

                Register(units);
                Register(abilities);
            }
        }

        public class ProponentProperty : Property
        {
            [SerializeField]
            protected EnergyProperty energy;
            public EnergyProperty Energy { get { return energy; } }
            [Serializable]
            public class EnergyProperty
            {
                [SerializeField]
                protected int initial;
                public int Initial { get { return initial; } }

                [SerializeField]
                protected IncreaseData increase;
                public IncreaseData Increase { get { return increase; } }
                [Serializable]
                public class IncreaseData
                {
                    [SerializeField]
                    protected float interval = 4f;
                    public float Interval { get { return interval; } }

                    [SerializeField]
                    protected int value = 200;
                    public int Value { get { return value; } }
                }
            }

            [SerializeField]
            protected BaseProperty _base;
            public BaseProperty Base { get { return _base; } }
            [Serializable]
            public class BaseProperty : Property
            {
                [SerializeField]
                protected BaseGraphicsData graphic;
                public BaseGraphicsData Graphic { get { return graphic; } }
            }

            public override void Configure()
            {
                base.Configure();

                Register(_base);
            }
        }

        public class Property : Core.Property
        {
            public LevelCore Level { get; protected set; }
            public virtual void Set(LevelCore reference) => Level = reference;

            public virtual void Register(Property property) => Level.Register(property);
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

        public override void Configure()
        {
            base.Configure();

            Register(player);
            Register(AI);
        }

        public virtual void Register(Property property)
        {
            property.Set(this);

            base.Register(property);
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