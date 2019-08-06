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
    public class Base : Entity, IModule<Proponent>
    {
        public Proponent Proponent { get; protected set; }

        [SerializeField]
        protected Transform entrance;
        public Transform Entrance { get { return entrance; } }

        public BaseUnitCreator UnitCreator { get; protected set; }

        new public abstract class Module : Module<Base>
        {
            public Base Base { get { return Data; } }

            public Proponent Proponent { get { return Base.Proponent; } }
        }

        public virtual void Configure(Proponent data)
        {
            Proponent = data;

            UnitCreator = Dependancy.Get<BaseUnitCreator>(gameObject);

            Modules.Configure(this);
        }

        public virtual void Init()
        {
            Modules.Init(this);
        }
    }
}