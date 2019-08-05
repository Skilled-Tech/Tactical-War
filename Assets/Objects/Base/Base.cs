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
	public class Base : Entity, IReference<Proponent>
	{
        public Proponent Proponent { get; protected set; }
        public void Init(Proponent data)
        {
            Proponent = data;
        }

        [SerializeField]
        protected Transform entrance;
        public Transform Entrance { get { return entrance; } }

        public BaseUnitCreator UnitCreator { get; protected set; }

        new public interface IReference : IReference<Base> { }
        new public abstract class Reference : Reference<Base>
        {
            public Base Base { get { return Data; } }

            public Proponent Proponent { get { return Base.Proponent; } }
        }

        protected override void Awake()
        {
            base.Awake();

            References.Init(this);

            UnitCreator = Dependancy.Get<BaseUnitCreator>(gameObject);
        }
    }
}