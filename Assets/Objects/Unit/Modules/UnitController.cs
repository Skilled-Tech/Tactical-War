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
    public abstract class UnitController : Unit.Module
    {
        public UnitNavigator Navigator { get { return Unit.Navigator; } }
        public UnitAttack Attack { get { return Unit.Attack; } }
        public int Index { get { return Unit.Index; } }

        public Proponent Leader { get { return Unit.Leader; } }
        public Base Base { get { return Leader.Base; } }

        public Proponent Enemy { get; protected set; }

        public bool IsAlive { get { return Unit.IsAlive; } }

        public Level Level { get { return Level.Instance; } }
        public LevelProponents Proponents { get { return Level.Proponents; } }

        public override void Init()
        {
            base.Init();

            Enemy = Proponents.GetEnemy(Leader);
        }
    }
}