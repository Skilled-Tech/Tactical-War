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
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(UIGrayscaleController))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(LayoutElement))]
    public class UnitUITemplate : UIElement, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        public Button Button { get; protected set; }

        public UIGrayscaleController GrayscaleController { get; protected set; }

        public CanvasGroup CanvasGroup { get; protected set; }

        public LayoutElement LayoutElement { get; protected set; }

        public RectTransform RectTransform { get; protected set; }

        public virtual void Init()
        {
            Button = GetComponent<Button>();
            Button.onClick.AddListener(Click);

            GrayscaleController = GetComponent<UIGrayscaleController>();
            GrayscaleController.Init();

            CanvasGroup = GetComponent<CanvasGroup>();

            LayoutElement = GetComponent<LayoutElement>();

            RectTransform = GetComponent<RectTransform>();
        }

        public UnitData Data { get; protected set; }
        public virtual void Set(UnitData data)
        {
            this.Data = data;

            icon.sprite = data.Icon;
        }

        public delegate void ClickDelegate(UnitUITemplate template, UnitData data);
        public event ClickDelegate OnClick;
        protected virtual void Click()
        {
            if (OnClick != null) OnClick(this, Data);
        }

        public delegate void DragDelegate(UnitUITemplate template, UnitData data, PointerEventData pointerData);

        public event DragDelegate DragBeginEvent;
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (DragBeginEvent != null) DragBeginEvent(this, Data, eventData);
        }

        public event DragDelegate DragEvent;
        public void OnDrag(PointerEventData eventData)
        {
            if (DragEvent != null) DragEvent(this, Data, eventData);
        }

        public event DragDelegate DragEndEvent;
        public void OnEndDrag(PointerEventData eventData)
        {
            if (DragEndEvent != null) DragEndEvent(this, Data, eventData);
        }
    }
}