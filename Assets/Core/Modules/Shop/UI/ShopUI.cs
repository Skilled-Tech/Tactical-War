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
    public class ShopUI : UIElement
    {
        [SerializeField]
        protected ShopSectionSelectionUI selection;
        public ShopSectionSelectionUI Selection { get { return selection; } }

        [SerializeField]
        protected ShopSectionUI section;
        public ShopSectionUI Section { get { return section; } }

        public BuyUI Buy => Core.Instance.UI.Buy;

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            selection.OnSelection += SelectionCallback;
            selection.Init();

            section.Init();
            section.OnSelection += SectionSelectionCallback;

            selection.Elements[0].Toggle.isOn = true;
        }

        void SectionSelectionCallback(ItemTemplate template)
        {
            Buy.Show(template);
        }

        void SelectionCallback(ShopSectionCore value)
        {
            section.Set(value);
        }
    }
}