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
	public class AbilitiesListUI : AbilitiesUI.Module
    {
        [SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        public AbilityUITemplate[] Templates { get; protected set; }

        public ScrollRect ScrollRect { get; protected set; }

        public RectTransform RectTransform { get; protected set; }

        public IList<AbilityTemplate> List { get { return Core.Items.Abilities.List; } }

        public override void Configure(AbilitiesUI data)
        {
            base.Configure(data);

            ScrollRect = GetComponent<ScrollRect>();

            RectTransform = GetComponent<RectTransform>();
        }

        public override void Init()
        {
            base.Init();

            Templates = new AbilityUITemplate[List.Count];

            for (int i = 0; i < List.Count; i++)
            {
                Templates[i] = CreateTemplate(List[i]);

                Templates[i].OnClick += TemplateClicked;
                Templates[i].DragBeginEvent += TemplateDragBegin;
                Templates[i].DragEvent += TemplateDrag;
                Templates[i].DragEndEvent += TemplateDragEnd;
            }
        }

        AbilityUITemplate CreateTemplate(AbilityTemplate data)
        {
            var instance = Instantiate(template, ScrollRect.content);

            instance.name = data.name + " Template";

            var script = instance.GetComponent<AbilityUITemplate>();

            script.Init();
            script.Set(data);

            return script;
        }

        public event AbilityUITemplate.ClickDelegate OnClick;
        void TemplateClicked(AbilityUITemplate template, AbilityTemplate data)
        {
            if (OnClick != null) OnClick(template, data);
        }

        public event AbilityUITemplate.DragDelegate OnTemplateDragBegin;
        void TemplateDragBegin(AbilityUITemplate template, AbilityTemplate unit, PointerEventData pointerData)
        {
            if (OnTemplateDragBegin != null) OnTemplateDragBegin(template, unit, pointerData);
        }

        public event AbilityUITemplate.DragDelegate OnTemplateDrag;
        void TemplateDrag(AbilityUITemplate template, AbilityTemplate unit, PointerEventData pointerData)
        {
            if (OnTemplateDrag != null) OnTemplateDrag(template, unit, pointerData);
        }

        public event AbilityUITemplate.DragDelegate OnTemplateDragEnd;
        void TemplateDragEnd(AbilityUITemplate template, AbilityTemplate unit, PointerEventData pointerData)
        {
            if (OnTemplateDragEnd != null) OnTemplateDragEnd(template, unit, pointerData);
        }
    }
}