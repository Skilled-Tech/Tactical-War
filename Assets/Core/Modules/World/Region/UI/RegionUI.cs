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

using TMPro;

namespace Game
{
	public class RegionUI : WorldUI.Element
	{
        [SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        public LevelsUIList Levels { get; protected set; }

        public RegionDifficultyContextUI Difficulty { get; protected set; }

        public class Element : UIElement , IModule<RegionUI>
        {
            public RegionUI Region { get; protected set; }

            public Core Core { get { return Core.Instance; } }
            public WorldCore World { get { return Core.World; } }
            public ScenesCore Scenes { get { return Core.Scenes; } }

            public virtual void Configure(RegionUI data)
            {
                this.Region = data;
            }

            public virtual void Init()
            {

            }
        }

        public override void Configure(WorldUI data)
        {
            base.Configure(data);

            Levels = Dependancy.Get<LevelsUIList>(gameObject);

            Difficulty = Dependancy.Get<RegionDifficultyContextUI>(gameObject);

            Modules.Configure(this);
        }

        public override void Init()
        {
            base.Init();

            Levels.OnSelect += OnLevelSelected;

            Modules.Init(this);
        }

        public RegionCore Region { get; protected set; }

        public virtual void Set(RegionCore data)
        {
            Region = data;

            label.text = data.name;

            Levels.Set(data);

            Show();
        }

        void OnLevelSelected(LevelCore level)
        {
            void Load(RegionDifficulty difficulty)
            {
                Difficulty.OnSelect -= Load;

                Region.Load(level, difficulty);
            }

            Difficulty.OnSelect += Load;
            Difficulty.For(level);
        }
    }
}