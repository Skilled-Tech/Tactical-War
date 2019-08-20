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
    public class UnitUpgrades : Unit.Module
    {
        public abstract class Module : Module<UnitUpgrades>
        {
            public UnitUpgrades Upgrades { get { return Data; } }

            public ProponentUpgradesContext Context { get { return Upgrades.Context; } }

            public Unit Unit { get { return Upgrades.Unit; } }
        }

        public List<Property> Properties { get; protected set; }

        public ProponentUpgradesContext Context { get; protected set; }

        public abstract class Property : Module
        {
            [SerializeField]
            protected UpgradeType target;
            public UpgradeType Target { get { return target; } }

            public ProponentUpgradeProperty UpgradeProperty { get; protected set; }

            public override void Init()
            {
                base.Init();

                UpgradeProperty = Context.Find(target);

                if(UpgradeProperty == null)
                {
                    enabled = false;
                    return;
                }
                else
                {
                    UpgradeProperty.OnUpgrade += OnUpgrade;

                    UpdateState();
                }
            }

            protected virtual void OnUpgrade()
            {
                UpdateState();
            }

            protected virtual void UpdateState()
            {
                
            }
        }

        public override void Configure(Unit data)
        {
            base.Configure(data);

            Properties = Dependancy.GetAll<Property>(gameObject);

            Modules.Configure(this);
        }

        public override void Init()
        {
            base.Init();

            Context = Leader.Base.Units.Upgrades.Contexts[Unit.Type];

            Modules.Init(this);
        }
    }
}