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
	public class UnitsUI : UIElement
	{
        public UnitsListUI List { get; protected set; }

        public UnitContextUI Context { get; protected set; }

        public UnitSelectionUI Selection { get; protected set; }

        public abstract class Module : UIElement, IModule<UnitsUI>
        {
            public UnitsUI UI { get; protected set; }

            public static Core Core { get { return Core.Instance; } }
            public static PopupUI Popup { get { return Core.UI.Popup; } }

            public static PlayerCore Player { get { return Core.Player; } }

            public virtual void Configure(UnitsUI data)
            {
                UI = data;
            }

            public virtual void Init()
            {
                
            }

            protected virtual void OnEnable()
            {

            }
        }

        public Core Core { get { return Core.Instance; } }

        void Awake()
        {
            List = Dependancy.Get<UnitsListUI>(gameObject);

            Context = Dependancy.Get<UnitContextUI>(gameObject);

            Selection = Dependancy.Get<UnitSelectionUI>(gameObject);

            Modules.Configure(this);
        }

        void Start()
        {
            List.OnClick += OnListUnitClicked;

            Modules.Init(this);
        }

        void OnListUnitClicked(UnitUITemplate template, UnitTemplate data)
        {
            Context.Show(data);
        }
    }
}