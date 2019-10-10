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
    public class ShopSectionSelectionUI : UIElement
    {
        [SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        public ShopSectionHandleUITemplate[] Elements { get; protected set; }

        [SerializeField]
        protected ToggleGroup toggleGroup;
        public ToggleGroup ToggleGroup { get { return toggleGroup; } }

        public Core Core { get { return Core.Instance; } }
        public ShopCore Shop { get { return Core.Shop; } }

        public virtual void Init()
        {
            Elements = new ShopSectionHandleUITemplate[Shop.Sections.Length];

            for (int i = 0; i < Elements.Length; i++)
            {
                var element = CreateTemplate(Shop.Sections[i]);

                Elements[i] = element;
            }
        }

        protected virtual ShopSectionHandleUITemplate CreateTemplate(ShopSectionCore section)
        {
            var instance = Instantiate(template, transform);

            var script = instance.GetComponent<ShopSectionHandleUITemplate>();

            script.Init(toggleGroup);
            script.Set(section);

            script.OnActivate += ElementActivatedCallback;

            return script;
        }

        public delegate void SelectionDelegate(ShopSectionCore section);
        public event SelectionDelegate OnSelection;
        void ElementActivatedCallback(ShopSectionHandleUITemplate template)
        {
            if (OnSelection != null) OnSelection(template.Section);
        }
    }
}