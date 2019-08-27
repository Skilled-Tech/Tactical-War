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
	public class Unit : Entity
	{
        public const string MenuPath = "Unit/";

        public UnitData Data { get; protected set; }

        public UnitType Type { get { return Data.Type; } }

        public UnitController Controller { get; protected set; }
        public UnitBody Body { get; protected set; }
        public UnitNavigator Navigator { get; protected set; }
        public UnitAttack Attack { get; protected set; }
        public UnitUpgrades Upgrades { get; protected set; }

        new public abstract class Module : Module<Unit>
        {
            public Unit Unit { get { return Reference; } }

            public UnitData Data { get { return Unit.Data; } }

            public UnitType Type { get { return Unit.Type; } }

            public UnitController Controller { get { return Unit.Controller; } }
            public UnitBody Body { get { return Unit.Body; } }
            public UnitNavigator Navigator { get { return Unit.Navigator; } }
            public UnitAttack Attack { get { return Unit.Attack; } }
            public UnitUpgrades Upgrades { get { return Unit.Upgrades; } }
            public Proponent Leader { get { return Unit.Leader; } }
            public Base Base { get { return Leader.Base; } }

            public int Index { get { return Unit.Index; } }
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

        public virtual void Configure(Proponent leader, UnitData data, UnitUpgradesController upgradesController)
        {
            this.Leader = leader;

            this.Data = data;

            Upgrades = Dependancy.Get<UnitUpgrades>(gameObject);
            Upgrades.Set(upgradesController);
        }

        protected override void Awake()
        {
            base.Awake();

            Controller = Dependancy.Get<UnitController>(gameObject);

            Body = Dependancy.Get<UnitBody>(gameObject);

            Navigator = Dependancy.Get<UnitNavigator>(gameObject);

            Attack = Dependancy.Get<UnitAttack>(gameObject);

            Bounds = GetComponent<Collider2D>().bounds;

            Modules.Configure(this);
        }

        protected override void Start()
        {
            base.Start();

            Health.Value = Health.Max = Data.Health;

            Modules.Init(this);
        }

        public override void TakeDamage(Entity damager, float value)
        {
            if(Upgrades.Defense == null)
            {

            }
            else
            {
                value = Mathf.Lerp(value, 0f, Upgrades.Defense.Percentage / 100f);
            }

            base.TakeDamage(damager, value);
        }

        protected override void Death(Entity damager)
        {
            base.Death(damager);

            Leader.Enemey.Funds.Add(Data.Deployment.Cost);

            Destroy(gameObject);
        }
    }
}