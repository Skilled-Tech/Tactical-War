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
	public class AbilitiesUI : UIElement
	{
        public AbilitiesListUI List { get; protected set; }

        public AbilitySelectionUI Selection { get; protected set; }

        public abstract class Module : UIElementModule<AbilitiesUI>
        {
            public AbilitiesUI UI => Reference;

            public static Core Core { get { return Core.Instance; } }
            public static PopupUI Popup { get { return Core.UI.Popup; } }

            public static PlayerCore Player { get { return Core.Player; } }

            protected virtual void OnEnable()
            {

            }
        }

        public Core Core { get { return Core.Instance; } }

        void Awake()
        {
            List = Dependancy.Get<AbilitiesListUI>(gameObject);

            Selection = Dependancy.Get<AbilitySelectionUI>(gameObject);

            Modules.Configure(this);
        }

        void Start()
        {
            List.OnClick += OnListElementClicked;

            Modules.Init(this);
        }

        void OnListElementClicked(AbilityUITemplate template, AbilityTemplate data)
        {
            if(Core.Player.Inventory.Contains(data))
            {

            }
            else
            {
                Core.UI.Buy.Show(data);
            }
        }
    }
}