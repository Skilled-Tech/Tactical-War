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
	public class ItemUpgradeTypeUITemplate : UIElement
	{
        [SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        public Toggle Toggle { get; protected set; }

        public virtual void Init(ToggleGroup togglegroup)
        {
            Toggle = GetComponent<Toggle>();

            Toggle.group = togglegroup;

            Toggle.onValueChanged.AddListener(ValueChangedCallback);
        }

        public ItemUpgradeType Context { get; protected set; }

		public virtual void Set(ItemUpgradeType context)
        {
            this.Context = context;

            context.Icon.ApplyTo(icon);
        }

        public delegate void SelectDelegate(ItemUpgradeType type);
        public event SelectDelegate OnSelect;
        protected virtual void ValueChangedCallback(bool isOn)
        {
            if(isOn)
            {
                if (OnSelect != null) OnSelect(Context);
            }
            else
            {

            }
        }
	}
}