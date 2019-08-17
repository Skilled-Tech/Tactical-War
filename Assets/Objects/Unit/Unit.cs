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
        [SerializeField]
        protected UnitData data;
        public UnitData Data { get { return data; } }

        public UnitController Controller { get; protected set; }
        public UnitBody Body { get; protected set; }
        public UnitNavigator Navigator { get; protected set; }
        public UnitAttack Attack { get; protected set; }

        new public abstract class Module : Module<Unit>
        {
            public Unit Unit { get { return Data; } }

            public UnitController Controller { get { return Unit.Controller; } }
            public UnitBody Body { get { return Unit.Body; } }
            public UnitNavigator Navigator { get { return Unit.Navigator; } }
            public UnitAttack Attack { get { return Unit.Attack; } }
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
        public virtual void Configure(Proponent leader)
        {
            this.Leader = leader;
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

            Modules.Init(this);
        }

        protected override void Death(Entity damager)
        {
            base.Death(damager);

            Leader.Enemey.Funds.Add(data.Cost);

            Destroy(gameObject);
        }
    }
}