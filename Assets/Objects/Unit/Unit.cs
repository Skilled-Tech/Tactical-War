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
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public class Unit : Entity
    {
        public const string MenuPath = Core.GameMenuPath + "Unit/";

        public UnitTemplate Template { get; protected set; }

        public UnitClass Class { get { return Template.Type; } }

        public UnitAudio audio { get; protected set; }

        public UnitController Controller { get; protected set; }
        public UnitBody Body { get; protected set; }
        public UnitNavigator Navigator { get; protected set; }
        public UnitAttack Attack { get; protected set; }
        public UnitUpgrades Upgrades { get; protected set; }
        public UnitSpeed Speed { get; protected set; }

        new public abstract class Module : Module<Unit>
        {
            public Unit Unit { get { return Reference; } }

            public UnitTemplate Template { get { return Unit.Template; } }

            public UnitClass Class { get { return Unit.Class; } }

            public UnitAudio audio => Unit.audio;

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
        public virtual void Configure(Proponent leader, IUnitSelectionData data)
        {
            this.Leader = leader;

            this.Template = data.Template;

            Upgrades = Dependancy.Get<UnitUpgrades>(gameObject);
            Upgrades.Set(data.Upgrades);
        }

        protected override void Awake()
        {
            base.Awake();

            audio = Dependancy.Get<UnitAudio>(gameObject);
            Controller = Dependancy.Get<UnitController>(gameObject);
            Body = Dependancy.Get<UnitBody>(gameObject);
            Navigator = Dependancy.Get<UnitNavigator>(gameObject);
            Attack = Dependancy.Get<UnitAttack>(gameObject);
            Speed = Dependancy.Get<UnitSpeed>(gameObject);

            Modules.Configure(this);
        }

        protected override void Start()
        {
            base.Start();

            Health.Value = Health.Max = Template.Health;

            Modules.Init(this);
        }

        protected virtual void Update()
        {
            Process();
        }

        public event Action OnProcess;
        protected virtual void Process()
        {
            if (OnProcess != null) OnProcess();
        }
        
        protected override void Death(Damage.Result result)
        {
            Leader.Enemy.Energy.Value += Template.Deployment.Cost / 2;

            base.Death(result);

            Destroy(gameObject);
        }
    }
}
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword