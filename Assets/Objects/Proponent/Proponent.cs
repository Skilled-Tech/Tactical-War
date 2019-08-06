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

        public abstract class Module : Module<Proponent>
        {
            public Proponent Proponent { get { return Data; } }
        }

        public Base Base { get; protected set; }

        protected AgeData age;
        public AgeData Age
        {
            get
            {
                return age;
            }
            set
            {
                age = value;
            }
        }

        public Level Level { get { return Level.Instance; } }

        protected virtual void Awake()
        {
            Funds = Dependancy.Get<ProponentFunds>(gameObject);

            Base = Dependancy.Get<Base>(gameObject);

            Modules.Configure(this);
        }

        protected virtual void Start()
        {
            Age = Level.Ages.List.First();

            Base.OnDeath += OnBaseDestroyed;

            Modules.Init(this);
        }

        private void OnBaseDestroyed(Entity damager)
        {
            
        }
    }
}