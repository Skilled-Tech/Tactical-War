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
	public class ItemUpgradeTypeUITemplate : UIElement
	{
        [SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        public Button Button { get; protected set; }

        public virtual void Init()
        {
            Button = GetComponent<Button>();

            Button.onClick.AddListener(ClickAction);
        }

        public ItemUpgradeType Context { get; protected set; }

		public virtual void Set(ItemUpgradeType context)
        {
            this.Context = context;

            context.Icon.ApplyTo(icon);
        }

        public delegate void ClickDelegate(ItemUpgradeType context);
        public event ClickDelegate OnClick;
        protected virtual void ClickAction()
        {
            if (OnClick != null) OnClick(Context);
        }
	}
}