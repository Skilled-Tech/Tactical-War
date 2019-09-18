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

using TMPro;

namespace Game
{
    [RequireComponent(typeof(Button))]
    public class LevelUITemplate : UIElement
	{
		[SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        [SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        public Button Button { get; protected set; }

        public UIGrayscaleController Grayscale { get; protected set; }

        public Core Core { get { return Core.Instance; } }

        public virtual void Init()
        {
            Button = GetComponent<Button>();
            Button.onClick.AddListener(ButtonClick);

            Grayscale = new UIGrayscaleController(this);
        }

        public LevelCore Level { get; protected set; }
        public virtual void Set(LevelCore data, int index)
        {
            Level = data;

            icon.sprite = Level.Icon;

            label.text = index.ToString();

            UpdateState();
        }

        void UpdateState()
        {
            Button.interactable = Level.Unlocked;

            Grayscale.Off = Button.interactable;
        }

        public event Action OnClick;
        void ButtonClick()
        {
            if (OnClick != null) OnClick();
        }

        void OnDestroy()
        {
            
        }
    }
}