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
        new public abstract class Module : Module<Unit>
        {
            public Unit Unit { get { return Data; } }
        }

        public UnitController Controller { get; protected set; }

        public Proponent Leader { get; protected set; }
        public virtual void Init(Proponent leader)
        {
            this.Leader = leader;
        }

        protected override void Awake()
        {
            base.Awake();

            Controller = Dependancy.Get<UnitController>(gameObject);

            Modules.Configure(this);
        }

        protected override void Start()
        {
            base.Start();

            Modules.Init(this);
        }
    }
}