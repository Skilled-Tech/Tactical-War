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
    public class ItemUITemplate : UIElement
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

        public ItemTemplate Template { get; protected set; }

        public virtual void Set(ItemTemplate template)
        {
            this.Template = template;

            template.Icon.ApplyTo(icon);
        }

        public delegate void ClickDelegate(ItemUITemplate template);
        public event ClickDelegate OnClick;
        void ClickAction()
        {
            if (OnClick != null) OnClick(this);
        }
    }
}