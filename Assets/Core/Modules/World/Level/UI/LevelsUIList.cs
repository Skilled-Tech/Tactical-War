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
	public class LevelsUIList : RegionUI.Element
    {
        [SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        public LevelUITemplate[] Templates { get; protected set; }

        public virtual void Set(RegionCore region)
        {
            Clear();

            Templates = new LevelUITemplate[region.Size];

            for (int i = 0; i < region.Size; i++)
            {
                var instance = Create(region[i], i);

                Templates[i] = instance;
            }
        }

        protected virtual void Clear()
        {
            if (Templates == null)
                return;

            for (int i = 0; i < Templates.Length; i++)
                Destroy(Templates[i].gameObject);
        }

        protected virtual LevelUITemplate Create(LevelCore level, int index)
        {
            var instance = Instantiate(template, transform);

            var script = instance.GetComponent<LevelUITemplate>();

            script.Init();
            script.Set(level, index);
            script.OnClick += ()=> TemplateClickedCallback(script);

            return script;
        }

        public delegate void SelectDelegate(LevelCore level);
        public event SelectDelegate OnSelect;
        void TemplateClickedCallback(LevelUITemplate template)
        {
            if (OnSelect != null) OnSelect(template.Level);
        }
    }
}