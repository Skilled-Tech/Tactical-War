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
    public class UnitController : Unit.Module
    {
        public Proponent Enemy { get { return Leader.Enemy; } }

        public bool IsAlive { get { return Unit.IsAlive; } }

        public Level Level { get { return Level.Instance; } }
        public LevelProponents Proponents { get { return Level.Proponents; } }

        public float PersonalSpace
        {
            get
            {
                return Unit.Bounds.extents.x;
            }
        }

        public override void Init()
        {
            base.Init();

            Unit.OnProcess += Process;
        }

        protected virtual void Process()
        {
            if (Index == 0)
                MoveTo(Attack.Target, Attack.Distance);
            else
                MoveTo(Base.Units.List[Index - 1], 1f);

            if (isMoving)
            {

            }
            else
            {
                if (Attack.CanPerform)
                    Attack.Perform();
            }
        }

        public bool isMoving { get; protected set; }
        bool MoveTo(Entity target, float stoppingDistance)
        {
            isMoving = !Navigator.MoveTo(target.Center, PersonalSpace + target.Bounds.extents.x + stoppingDistance);

            return !isMoving;
        }
    }
}