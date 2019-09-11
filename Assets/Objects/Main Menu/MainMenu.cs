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
    [DefaultExecutionOrder(ExecutionOrder)]
    public class MainMenu : UIElement
    {
        public const int ExecutionOrder = -200;

        public static MainMenu Instance { get; protected set; }

        [SerializeField]
        protected UIElement title;
        public UIElement Title { get { return title; } }

        [SerializeField]
        protected UIElement start;
        public UIElement Start { get { return start; } }

        [SerializeField]
        protected UIElement levels;
        public UIElement Levels { get { return levels; } }

        [SerializeField]
        protected UnitsUI units;
        public UnitsUI Units { get { return units; } }

        [SerializeField]
        protected UIElement credits;
        public UIElement Credits { get { return credits; } }

        [SerializeField]
        protected PopupUI popup;
        public PopupUI Popup { get { return popup; } }

        void Awake()
        {
            if(!PlayFab.PlayFabClientAPI.IsClientLoggedIn())
            {
                Core.Instance.Scenes.Load(Core.Instance.Scenes.Login.Name);
                return;
            }

            Instance = this;

            title.Show();

            start.Hide();
            levels.Hide();
            units.Hide();
            credits.Hide();
        }
    }
}