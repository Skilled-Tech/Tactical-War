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
using UnityEngine.EventSystems;

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

        public RectTransform RectTransform { get; protected set; }

        public IList<UnitTemplate> List { get { return Core.Items.Units.List; } }

        public override void Configure(UnitsUI data)
        {
            base.Configure(data);

            ScrollRect = GetComponent<ScrollRect>();

            RectTransform = GetComponent<RectTransform>();
        }

        public override void Init()
        {
            base.Init();

            Templates = new UnitUITemplate[List.Count];

            for (int i = 0; i < List.Count; i++)
            {
                Templates[i] = CreateTemplate(List[i]);

                Templates[i].OnClick += TemplateClicked;
                Templates[i].DragBeginEvent += TemplateDragBegin;
                Templates[i].DragEvent += TemplateDrag;
                Templates[i].DragEndEvent += TemplateDragEnd;
            }
        }

        UnitUITemplate CreateTemplate(UnitTemplate data)
        {
            var instance = Instantiate(template, ScrollRect.content);

            instance.name = data.name + " Template";

            var script = instance.GetComponent<UnitUITemplate>();

            script.Init();
            script.Set(data);

            return script;
        }

        public event UnitUITemplate.ClickDelegate OnClick;
        void TemplateClicked(UnitUITemplate template, UnitTemplate data)
        {
            if (OnClick != null) OnClick(template, data);
        }

        public event UnitUITemplate.DragDelegate OnTemplateDragBegin;
        void TemplateDragBegin(UnitUITemplate template, UnitTemplate unit, PointerEventData pointerData)
        {
            if (OnTemplateDragBegin != null) OnTemplateDragBegin(template, unit, pointerData);
        }

        public event UnitUITemplate.DragDelegate OnTemplateDrag;
        void TemplateDrag(UnitUITemplate template, UnitTemplate unit, PointerEventData pointerData)
        {
            if (OnTemplateDrag != null) OnTemplateDrag(template, unit, pointerData);
        }

        public event UnitUITemplate.DragDelegate OnTemplateDragEnd;
        void TemplateDragEnd(UnitUITemplate template, UnitTemplate unit, PointerEventData pointerData)
        {
            if (OnTemplateDragEnd != null) OnTemplateDragEnd(template, unit, pointerData);
        }
    }
}