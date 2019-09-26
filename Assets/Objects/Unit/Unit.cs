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
    public class Unit : Entity
    {
        public const string MenuPath = "Unit/";

        public UnitTemplate Template { get; protected set; }

        public UnitClass Class { get { return Template.Type; } }

        public UnitController Controller { get; protected set; }
        public UnitBody Body { get; protected set; }
        public UnitNavigator Navigator { get; protected set; }
        public UnitAttack Attack { get; protected set; }
        public UnitUpgrades Upgrades { get; protected set; }

        new public abstract class Module : Module<Unit>
        {
            public Unit Unit { get { return Reference; } }

            public UnitTemplate Template { get { return Unit.Template; } }

            public UnitClass Class { get { return Unit.Class; } }

            public UnitController Controller { get { return Unit.Controller; } }

            public UnitBody Body { get { return Unit.Body; } }

            public UnitNavigator Navigator { get { return Unit.Navigator; } }

            public UnitAttack Attack { get { return Unit.Attack; } }

            public UnitUpgrades Upgrades { get { return Unit.Upgrades; } }

            public Proponent Leader { get { return Unit.Leader; } }

            public Base Base { get { return Leader.Base; } }

            public int Index { get { return Unit.Index; } }

            public Core Core { get { return Core.Instance; } }
        }

        protected int _index;
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }

        public Proponent Leader { get; protected set; }
        public virtual void Configure(Proponent leader, UnitTemplate template, ItemUpgradeData upgreades)
        {
            this.Leader = leader;

            this.Template = template;

            Upgrades = Dependancy.Get<UnitUpgrades>(gameObject);
            Upgrades.Set(upgreades);
        }

        protected override void Awake()
        {
            base.Awake();

            Controller = Dependancy.Get<UnitController>(gameObject);

            Body = Dependancy.Get<UnitBody>(gameObject);

            Navigator = Dependancy.Get<UnitNavigator>(gameObject);

            Attack = Dependancy.Get<UnitAttack>(gameObject);

            Modules.Configure(this);
        }

        protected override void Start()
        {
            base.Start();

            Health.Value = Health.Max = Template.Health;

            Modules.Init(this);
        }

        protected override void Death(Damage.Result result)
        {
            Leader.Enemy.Energy.Value += Template.Deployment.Cost;

            base.Death(result);

            Destroy(gameObject);
        }
    }
}