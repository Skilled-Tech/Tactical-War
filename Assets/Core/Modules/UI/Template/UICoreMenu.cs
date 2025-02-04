﻿using System;
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
	public class UICoreMenu : UIElement
	{
        [SerializeField]
        protected OptionsMenu optinosMenu;
        public OptionsMenu OptionsMenu { get { return optinosMenu; } }

        [SerializeField]
        protected RewardsUI rewards;
        public RewardsUI Rewards { get { return rewards; } }

        [SerializeField]
        protected PopupUI popup;
        public PopupUI Popup { get { return popup; } }

        [SerializeField]
        protected BuyUI buy;
        public BuyUI Buy { get { return buy; } }

        [SerializeField]
        protected DialogUI dialog;
        public DialogUI Dialog { get { return dialog; } }

        [SerializeField]
        protected FaderUI fader;
        public FaderUI Fader { get { return fader; } }

        public Core Core => Core.Instance;

        public virtual void Init()
        {
            buy.Init();
            rewards.Init();

            optinosMenu.Hide();
            rewards.Hide();
            popup.Hide();
            buy.Hide();

            fader.Show();
            fader.Value = 0f;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
                Core.Localization.Progress();
#endif
        }
    }
}