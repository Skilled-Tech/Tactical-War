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

        public UIGrayscaleController GrayscaleController { get; protected set; }

        public LevelData Element { get; protected set; }

        public Core Core { get { return Core.Instance; } }

        public virtual void Set(LevelData data, string text)
        {
            Element = data;

            Element.OnChange += UpdateState;

            icon.sprite = Element.Icon;

            label.text = text;

            Button = GetComponent<Button>();
            Button.onClick.AddListener(ButtonClick);

            GrayscaleController = new UIGrayscaleController(this);

            UpdateState();
        }

        void UpdateState()
        {
            Button.interactable = Element.Unlocked;

            GrayscaleController.On = !Button.interactable;
        }

        void ButtonClick()
        {
            Core.Levels.Load(Element);
        }

        void OnDestroy()
        {
            if (Element != null) Element.OnChange -= UpdateState;
        }
    }
}