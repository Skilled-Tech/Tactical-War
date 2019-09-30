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
    [RequireComponent(typeof(CanvasGroup))]
    public class ItemUpgradeTypeUITemplate : UIElement
	{
        [SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        public Toggle Toggle { get; protected set; }

        public RectTransform RectTransform { get; protected set; }

        public Vector2 Size { get { return RectTransform.sizeDelta; } }

        public CanvasGroup CanvasGroup { get; protected set; }

        public RateTransition RateTransition { get; protected set; }

        public virtual void Init(ToggleGroup togglegroup)
        {
            Toggle = GetComponent<Toggle>();
            Toggle.group = togglegroup;
            Toggle.onValueChanged.AddListener(ValueChangedCallback);

            RectTransform = GetComponent<RectTransform>();

            CanvasGroup = GetComponent<CanvasGroup>();

            RateTransition = new RateTransition(this, 0.5f);
            RateTransition.OnValueChanged += RateTransitionValueCallback;
            RateTransition.Speed = 15f;
            RateTransition.To(0f);
        }

        void RateTransitionValueCallback(float rate)
        {
            CanvasGroup.alpha = Mathf.Lerp(0.65f, 1f, rate);

            var yPosition = Mathf.Lerp(Size.y * -0.15f, Size.y * 0.15f, rate);

            RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x, yPosition);
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

                RateTransition.To(1f);
            }
            else
            {
                RateTransition.To(0f);
            }
        }
	}
}