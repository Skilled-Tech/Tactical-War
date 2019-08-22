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
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(UIGrayscaleController))]
	public class UnitUITemplate : UIElement
	{
        [SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        public Button Button { get; protected set; }

        public UIGrayscaleController GrayscaleController { get; protected set; }

        public UnitData Data { get; protected set; }
        public virtual void Set(UnitData data)
        {
            this.Data = data;

            icon.sprite = data.Icon;
        }

        protected virtual void Awake()
        {
            Button = GetComponent<Button>();
            Button.onClick.AddListener(Click);

            GrayscaleController = GetComponent<UIGrayscaleController>();
            GrayscaleController.Init();
        }

        public delegate void ClickDelegate(UnitUITemplate template, UnitData data);
        public event ClickDelegate OnClick;
        protected virtual void Click()
        {
            if (OnClick != null) OnClick(this, Data);
        }
    }
}