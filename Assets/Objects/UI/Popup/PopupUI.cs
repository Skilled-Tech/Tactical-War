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
	public class PopupUI : UIElement
	{
		[SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        public string Text { get { return label.text; } set { label.text = value; } }

        [SerializeField]
        protected Button button;
        public Button Button { get { return button; } }

        [SerializeField]
        protected TMP_Text instructions;
        public TMP_Text Instructions { get { return instructions; } }

        protected virtual void Awake()
        {
            button.onClick.AddListener(OnButton);
        }

        public virtual void Show(string text)
        {
            Show(text, null, null);
        }
        public virtual void Show(string text, Action action, string instructions)
        {
            label.text = text;

            button.gameObject.SetActive(action != null);
            this.action = action;

            if (action == null)
            {

            }
            else
            {
                this.instructions.text = instructions;
            }

            Show();
        }

        Action action;
        void OnButton()
        {
            if (action != null) action();
        }
    }
}