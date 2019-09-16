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
                var instance = Create(region[i]);

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

        protected virtual LevelUITemplate Create(LevelCore level)
        {
            var instance = Instantiate(template);

            var script = instance.GetComponent<LevelUITemplate>();

            script.Init();
            script.Set(level);
            script.OnClick += ()=> TemplateClickedCallback(script);

            return script;
        }

        public event Action<LevelUITemplate> OnSelect;
        void TemplateClickedCallback(LevelUITemplate template)
        {
            if (OnSelect != null) OnSelect(template);
        }
    }
}