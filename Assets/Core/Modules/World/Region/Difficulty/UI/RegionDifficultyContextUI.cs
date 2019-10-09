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
	public class RegionDifficultyContextUI : RegionUI.Element
	{
        [SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        [SerializeField]
        protected RectTransform panel;
        public RectTransform Panel { get { return panel; } }

        public List<RegionDifficultyUITemplate> Elements { get; protected set; }

        public override void Configure(RegionUI data)
        {
            base.Configure(data);

            Elements = new List<RegionDifficultyUITemplate>();
        }

        public override void Init()
        {
            base.Init();

            Create(World.Difficulty.List);
        }

        public virtual void For(LevelCore level)
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].Interactable = level.UnlockedOn(Elements[i].Difficulty);
            }

            Show();
        }

        protected virtual void Create(IList<RegionDifficulty> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var element = CreateTemplate(list[i]);

                Elements.Add(element);
            }
        }
        protected virtual RegionDifficultyUITemplate CreateTemplate(RegionDifficulty difficulty)
        {
            var instance = Instantiate(template, panel);

            var script = instance.GetComponent<RegionDifficultyUITemplate>();

            script.Init();
            script.Set(difficulty);

            script.OnClick += OnElementClicked;

            return script;
        }

        public delegate void SelectDelegate(RegionDifficulty difficulty);
        public event SelectDelegate OnSelect;
        void OnElementClicked(RegionDifficultyUITemplate UITemplate)
        {
            if (OnSelect != null) OnSelect(UITemplate.Difficulty);

            Hide();
        }
    }
}