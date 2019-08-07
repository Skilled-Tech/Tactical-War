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
	public class Proponent : MonoBehaviour
	{
        public ProponentFunds Funds { get; protected set; }

        public ProponentAge Age { get; protected set; }

        public ProponentAbility Ability { get; protected set; }

        public abstract class Module : Module<Proponent>
        {
            public Proponent Proponent { get { return Data; } }

            public Level Level { get { return Level.Instance; } }
        }

        public Base Base { get; protected set; }
        public BaseUnits Units { get { return Base.Units; } }

        public Level Level { get { return Level.Instance; } }

        public Proponent Enemey { get; protected set; }

        protected virtual void Awake()
        {
            Funds = Dependancy.Get<ProponentFunds>(gameObject);

            Age = Dependancy.Get<ProponentAge>(gameObject);

            Ability = Dependancy.Get<ProponentAbility>(gameObject);

            Modules.Configure(this);

            Base = Dependancy.Get<Base>(gameObject);

            Enemey = Level.Proponents.GetEnemy(this);
        }

        protected virtual void Start()
        {
            Age.Set(Level.Ages.List.First());

            Base.OnDeath += OnBaseDestroyed;

            Modules.Init(this);
        }

        protected virtual void OnBaseDestroyed(Entity damager)
        {
            
        }
    }
}