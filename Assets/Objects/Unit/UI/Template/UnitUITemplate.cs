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

        public Core Core { get { return Core.Instance; } }
        public PlayerCore Player { get { return Core.Player; } }

        public virtual void Init()
        {
            Button = GetComponent<Button>();
            Button.onClick.AddListener(Click);

            GrayscaleController = new UIGrayscaleController(this);

            Player.Inventory.OnChange += UpdateState;

            CanvasGroup = GetComponent<CanvasGroup>();

            LayoutElement = GetComponent<LayoutElement>();

            RectTransform = GetComponent<RectTransform>();
        }

        public UnitTemplate Template { get; protected set; }
        public virtual void Set(UnitTemplate template)
        {
            this.Template = template;

            template.Icon.ApplyTo(icon);

            UpdateState();
        }

        protected virtual void UpdateState()
        {
            if (Template == null)
            {

            }
            else
            {
                GrayscaleController.Off = Player.Inventory.Contains(Template.CatalogItem);
            }
        }

        public delegate void ClickDelegate(UnitUITemplate template, UnitTemplate data);
        public event ClickDelegate OnClick;
        protected virtual void Click()
        {
            if (OnClick != null) OnClick(this, Template);
        }

        #region Drag
        public delegate void DragDelegate(UnitUITemplate template, UnitTemplate data, PointerEventData pointerData);

        public event DragDelegate DragBeginEvent;
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (DragBeginEvent != null) DragBeginEvent(this, Template, eventData);
        }

        public event DragDelegate DragEvent;
        public void OnDrag(PointerEventData eventData)
        {
            if (DragEvent != null) DragEvent(this, Template, eventData);
        }

        public event DragDelegate DragEndEvent;
        public void OnEndDrag(PointerEventData eventData)
        {
            if (DragEndEvent != null) DragEndEvent(this, Template, eventData);
        }
        #endregion

        void OnDestroy()
        {
            Player.Inventory.OnChange -= UpdateState;
        }
    }
}