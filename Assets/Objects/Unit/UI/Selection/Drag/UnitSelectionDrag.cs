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
	public class UnitSelectionDrag : MonoBehaviour
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

        public Core Core { get { return Core.Instance; } }

        void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        void Start()
        {
            list.OnTemplateDragBegin += ListTemplateDragBegin;
            list.OnTemplateDrag += ListTemplateDrag;
            list.OnTemplateDragEnd += ListTemplateDragEnd;
        }

        UnitUITemplate CreateTemplate(UnitUITemplate source)
        {
            var instance = Instantiate(template, RectTransform);

            instance.name = source.Data.name + " Drag Template";

            var script = instance.GetComponent<UnitUITemplate>();

            script.Init();
            script.Set(source.Data);

            script.LayoutElement.ignoreLayout = true;
            script.CanvasGroup.blocksRaycasts = false;

            script.RectTransform.sizeDelta = source.RectTransform.sizeDelta * 0.8f;

            return script;
        }

        void ListTemplateDragBegin(UnitUITemplate template, UnitData unit, PointerEventData pointerData)
        {
            if (Instance == null)
            {
                Core.Player.Units.Selection.Context = template.Data;

                Instance = CreateTemplate(template);

                SetTemplatePosition(pointerData);
            }
            else
            {

            }
        }

        void ListTemplateDrag(UnitUITemplate template, UnitData unit, PointerEventData pointerData)
        {
            if (Instance == null)
            {

            }
            else
            {
                if (Instance.Data == template.Data)
                {
                    SetTemplatePosition(pointerData);
                }
            }
        }

        void ListTemplateDragEnd(UnitUITemplate template, UnitData unit, PointerEventData pointerData)
        {
            if (Instance == null)
            {

            }
            else
            {
                Destroy(Instance.gameObject);

                Instance = null;

                Core.Player.Units.Selection.Context = null;
            }
        }
    }
}