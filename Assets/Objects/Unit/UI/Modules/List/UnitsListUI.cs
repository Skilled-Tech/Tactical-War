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
	public class UnitsListUI : UnitsUI.Module
    {
        [SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        public UnitUITemplate[] Templates { get; protected set; }

        public ScrollRect ScrollRect { get; protected set; }

        public IList<UnitData> List { get { return Core.Player.Units.List; } }

        public override void Configure(UnitsUI data)
        {
            base.Configure(data);

            ScrollRect = GetComponent<ScrollRect>();
        }

        public override void Init()
        {
            base.Init();

            Templates = new UnitUITemplate[List.Count];

            for (int i = 0; i < List.Count; i++)
            {
                Templates[i] = CreateTemplate(List[i]);

                Templates[i].OnClick += TemplateClicked;
            }
        }

        UnitUITemplate CreateTemplate(UnitData data)
        {
            var instance = Instantiate(template, ScrollRect.content);

            instance.name = data.name + " Template";

            var script = instance.GetComponent<UnitUITemplate>();

            script.Set(data);

            return script;
        }

        public event UnitUITemplate.ClickDelegate OnUnitClicked;
        void TemplateClicked(UnitUITemplate template, UnitData data)
        {
            if (OnUnitClicked != null) OnUnitClicked(template, data);
        }
    }
}