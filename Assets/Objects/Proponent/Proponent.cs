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

        public interface IReference : IReference<Proponent> { }
        public abstract class Reference : Reference<Proponent>
        {
            public Proponent Proponent { get { return Data; } }
        }

        public Base Base { get; protected set; }

        protected Age age;
        public Age Age
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
            References.Init(this);

            Age = Level.Ages.List.First();

            Funds = Dependancy.Get<ProponentFunds>(gameObject);

            Base = Dependancy.Get<Base>(gameObject);

            Base.OnDeath += OnBaseDestroyed;
        }

        protected virtual void Start()
        {
            
        }

        private void OnBaseDestroyed(Entity damager)
        {
            
        }
    }
}