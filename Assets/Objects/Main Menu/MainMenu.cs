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
        protected WorldUI world;
        public WorldUI World { get { return world; } }

        [SerializeField]
        protected InventoryUI inventory;
        public InventoryUI Inventory { get { return inventory; } }

        [SerializeField]
        protected ShopUI shop;
        public ShopUI Shop { get { return shop; } }

        [SerializeField]
        protected UnitsUI units;
        public UnitsUI Units { get { return units; } }

        [SerializeField]
        protected UIElement credits;
        public UIElement Credits { get { return credits; } }

        public virtual void ForAll(Action<UIElement> action)
        {
            action(title);

            action(start);

            action(world);

            action(inventory);

            action(shop);

            action(units);

            action(credits);
        }

        public Core Core { get { return Core.Instance; } }
        public PopupUI Popup { get { return Core.UI.Popup; } }

        void Awake()
        {
            Core.PlayFab.EnsureActivation();

            Instance = this;

            void HideAllButTitle(UIElement element) => element.Visibile = element == title;

            ForAll(HideAllButTitle);
        }
    }
}