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
	public class RegionDifficultyUITemplate : UIElement
	{
		[SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        public Button Button { get; protected set; }

        public bool Interactable
        {
            set
            {
                Button.interactable = value;

                label.color = value ? Color.white : Color.Lerp(Color.white, Color.black, 0.75f);

                label.SetStrikeThrough(value);
            }
        }

        public virtual void Init()
        {
            Button = GetComponent<Button>();

            Button.onClick.AddListener(ClickAction);
        }

        public RegionDifficulty Difficulty { get; protected set; }
        public virtual void Set(RegionDifficulty difficulty)
        {
            this.Difficulty = difficulty;

            label.text = difficulty.name;
        }

        public delegate void ClickDelegate(RegionDifficultyUITemplate UITemplate);
        public event ClickDelegate OnClick;
        protected virtual void ClickAction()
        {
            if (OnClick != null) OnClick(this);
        }
    }
}