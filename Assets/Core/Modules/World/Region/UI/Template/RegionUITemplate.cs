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
	public class RegionUITemplate : UIElement
	{
		[SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        public Button Button { get; protected set; }

        public UIGrayscaleController Grayscale { get; protected set; }

        public virtual void Init()
        {
            Button = GetComponent<Button>();
            Button.onClick.AddListener(ClickAction);

            Grayscale = new UIGrayscaleController(this);
        }

        public RegionCore Region { get; protected set; }
        public virtual void Set(RegionCore region)
        {
            icon.sprite = region.Icon;

            UpdateState();
        }

        protected virtual void UpdateState()
        {
            Button.interactable = Region.Unlocked;

            Grayscale.Off = Button.interactable;
        }

        public event Action OnClick;
        void ClickAction()
        {
            if (OnClick != null) OnClick();
        }
    }
}