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
    [RequireComponent(typeof(ScrollRect))]
	public class LevelsUIList : UIElement
	{
        [SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        public LevelUITemplate[] Templates { get; protected set; }

        public ScrollRect ScrollRect { get; protected set; }

        public Core Core { get { return Core.Instance; } }
        public ScenesCore Scenes { get { return Core.Scenes; } }
        public LevelsCore Levels { get { return Core.Levels; } }

        void Awake()
        {
            ScrollRect = GetComponent<ScrollRect>();

            Templates = new LevelUITemplate[Levels.Count];

            for (int i = 0; i < Levels.Count; i++)
            {
                var instance = Create(Levels[i], i);

                Templates[i] = instance;
            }
        }

        LevelUITemplate Create(LevelData element, int index)
        {
            var instance = Instantiate(template, ScrollRect.content);

            instance.name = element.Scene.Name;

            var script = instance.GetComponent<LevelUITemplate>();

            script.Set(element, (index + 1).ToString());

            return script;
        }
    }
}