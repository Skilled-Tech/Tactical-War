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
    [DefaultExecutionOrder(ExecutionOrder)]
    public abstract class Proponent : MonoBehaviour
    {
        public const int ExecutionOrder = Level.ExecutionOrder + 1;

        public int Layer { get { return gameObject.layer; } }

        public ProponentAbilities Abilities { get; protected set; }
        public ProponentUnits Units { get; protected set; }
        public ProponentEnergy Energy { get; protected set; }
        public abstract class Module : Module<Proponent>
        {
            public Proponent Proponent { get { return Reference; } }

            public Level Level { get { return Level.Instance; } }
        }

        public abstract LevelCore.ProponentProperty LevelData { get; }

        public Base Base { get; protected set; }

        public Proponent Enemy { get; protected set; }

        public Level Level { get { return Level.Instance; } }

        protected virtual void Awake()
        {
            Abilities = Dependancy.Get<ProponentAbilities>(gameObject);

            Units = Dependancy.Get<ProponentUnits>(gameObject);

            Energy = Dependancy.Get<ProponentEnergy>(gameObject);

            Modules.Configure(this);

            Base = Dependancy.Get<Base>(gameObject);

            Enemy = Level.Proponents.GetOther(this);
        }

        protected virtual void Start()
        {
            Base.OnDeath += OnBaseDestroyed;

            Modules.Init(this);
        }

        void OnBaseDestroyed(Damage.Result result)
        {
            DeclareDefeat();
        }

        public event Action OnDefeat;
        protected virtual void DeclareDefeat()
        {
            if (OnDefeat != null) OnDefeat();
        }
    }
}