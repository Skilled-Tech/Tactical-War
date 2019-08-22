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

        public UnitsContextUI Context { get; protected set; }

        public UnitsUpgradeUI Upgrade { get; protected set; }

        public abstract class Module : UIElement, IModule<UnitsUI>
        {
            public UnitsUI UI { get; protected set; }

            public Core Core { get { return Core.Instance; } }

            public virtual void Configure(UnitsUI data)
            {
                UI = data;
            }

            public virtual void Init()
            {
                
            }
        }

        public Core Core { get { return Core.Instance; } }

        void Awake()
        {
            List = Dependancy.Get<UnitsListUI>(gameObject);

            Context = Dependancy.Get<UnitsContextUI>(gameObject);

            Upgrade = Dependancy.Get<UnitsUpgradeUI>(gameObject);

            Modules.Configure(this);
        }

        void Start()
        {
            List.OnUnitClicked += OnListUnitClicked;

            Modules.Init(this);
        }

        void OnListUnitClicked(UnitUITemplate template, UnitData data)
        {
            Context.Set(data);
        }
    }
}