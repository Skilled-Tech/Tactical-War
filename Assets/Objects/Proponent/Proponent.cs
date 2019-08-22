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
	public abstract class Proponent : MonoBehaviour
	{
        public int Layer { get { return gameObject.layer; } }

        public abstract Funds Funds { get; }

        public IList<UnitData> UnitSelection { get { return Core.Instance.Units.Roster.List; } }

        public ProponentAbility Ability { get; protected set; }
        public abstract class Module : Module<Proponent>
        {
            public Proponent Proponent { get { return Reference; } }

            public Level Level { get { return Level.Instance; } }
        }

        public Base Base { get; protected set; }

        public Proponent Enemey { get; protected set; }

        public Level Level { get { return Level.Instance; } }

        protected virtual void Awake()
        {
            Ability = Dependancy.Get<ProponentAbility>(gameObject);

            Modules.Configure(this);

            Base = Dependancy.Get<Base>(gameObject);

            Enemey = Level.Proponents.GetOther(this);
        }

        protected virtual void Start()
        {
            Base.OnDeath += OnBaseDestroyed;

            Modules.Init(this);
        }

        protected virtual void OnBaseDestroyed(Entity damager)
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