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
    [RequireComponent(typeof(Toggle))]
	public class ShopSectionHandleUITemplate : UIElement
	{
		[SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        public Toggle Toggle { get; protected set; }

        public virtual void Init(ToggleGroup toggleGroup)
        {
            Toggle = GetComponent<Toggle>();
            Toggle.onValueChanged.AddListener(ToggleAction);
            Toggle.group = toggleGroup;
        }

        public ShopSectionCore Section { get; protected set; }

        public virtual void Set(ShopSectionCore section)
        {
            this.Section = section;

            section.Icon.ApplyTo(icon);
        }

        public delegate void ActivateDelegate(ShopSectionHandleUITemplate template);
        public event ActivateDelegate OnActivate;
        protected virtual void ToggleAction(bool isOn)
        {
            if(isOn)
            {
                if (OnActivate != null) OnActivate(this);
            }
        }
    }
}