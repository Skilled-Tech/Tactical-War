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
	public class UnitSelectionDragUI : UnitsUI.Module
    {
        [SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        [SerializeField]
        protected UnitsListUI list;
        public UnitsListUI List { get { return list; } }

        public RectTransform RectTransform { get; protected set; }

        public UnitUITemplate Instance { get; protected set; }
        protected virtual void SetTemplatePosition(PointerEventData pointerData)
        {
            Vector2 localPoint;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, pointerData.position, pointerData.pressEventCamera, out localPoint))
                Instance.transform.localPosition = localPoint;
        }

        public override void Configure(UnitsUI data)
        {
            base.Configure(data);

            RectTransform = GetComponent<RectTransform>();
        }

        public override void Init()
        {
            base.Init();

            list.OnTemplateDragBegin += ListTemplateDragBegin;
            list.OnTemplateDrag += ListTemplateDrag;
            list.OnTemplateDragEnd += ListTemplateDragEnd;
        }

        UnitUITemplate CreateTemplate(UnitUITemplate source)
        {
            var instance = Instantiate(template, RectTransform);

            instance.name = source.Template.name + " Drag Template";

            var script = instance.GetComponent<UnitUITemplate>();

            script.Init();
            script.Set(source.Template);

            script.LayoutElement.ignoreLayout = true;
            script.CanvasGroup.blocksRaycasts = false;

            script.RectTransform.sizeDelta = source.RectTransform.sizeDelta * 0.8f;

            return script;
        }

        public event Action OnDragBegin;
        void ListTemplateDragBegin(UnitUITemplate template, UnitTemplate unit, PointerEventData pointerData)
        {
            if (Instance == null)
            {
                if(template.Data.Unlocked)
                {
                    Core.Player.Units.Selection.Context = template.Template;

                    Instance = CreateTemplate(template);

                    SetTemplatePosition(pointerData);

                    if (OnDragBegin != null) OnDragBegin();
                }
            }
            else
            {

            }
        }

        void ListTemplateDrag(UnitUITemplate template, UnitTemplate unit, PointerEventData pointerData)
        {
            if (Instance == null)
            {

            }
            else
            {
                if (Instance.Template == template.Template)
                {
                    SetTemplatePosition(pointerData);
                }
            }
        }

        public event Action OnDragEnd;
        void ListTemplateDragEnd(UnitUITemplate template, UnitTemplate unit, PointerEventData pointerData)
        {
            if (Instance == null)
            {

            }
            else
            {
                Destroy(Instance.gameObject);

                Instance = null;

                Core.Player.Units.Selection.Context = null;

                if (OnDragEnd != null) OnDragEnd();
            }
        }
    }
}