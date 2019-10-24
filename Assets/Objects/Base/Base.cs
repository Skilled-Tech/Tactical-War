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
    [DefaultExecutionOrder(Proponent.ExecutionOrder + 1)]
    public class Base : Entity, IModule<Proponent>
    {
        public const string MenuPath = Core.GameMenuPath + "Base/";

        public Proponent Proponent { get; protected set; }

        public BaseUnits Units { get; protected set; }

        public BaseTower Tower { get; protected set; }

        public BaseGraphic Graphic { get; protected set; }

        public Transform Entrance { get { return Units.Creator.transform; } }

        new public abstract class Module : Module<Base>
        {
            public const string MenuPath = Base.MenuPath + "Modules/";

            public Base Base { get { return Reference; } }

            public Proponent Proponent { get { return Base.Proponent; } }
        }

        public LevelCore.ProponentData.BaseData LevelData => Proponent.LevelData.Base;

        public virtual void Configure(Proponent data)
        {
            Proponent = data;

            Units = Dependancy.Get<BaseUnits>(gameObject);
            Tower = Dependancy.Get<BaseTower>(gameObject);
            Graphic = Dependancy.Get<BaseGraphic>(gameObject);

            Modules.Configure(this);
        }

        public virtual void Init()
        {
            Modules.Init(this);
        }
    }
}