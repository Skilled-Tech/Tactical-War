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
        public Proponent Enemy { get { return Leader.Enemy; } }

        public bool IsAlive { get { return Unit.IsAlive; } }

        public Level Level { get { return Level.Instance; } }
        public LevelProponents Proponents { get { return Level.Proponents; } }

        public Entity Target { get; protected set; }

        public bool isMoving { get; protected set; }
        public bool isAttacking { get { return Attack.IsProcessing; } }

        void Update()
        {
            if(isAttacking)
            {

            }
            else
            {
                if (Index < Attack.Range.Value || true)
                {
                    if (Enemy.Base.Units.Count == 0)
                        Target = Enemy.Base;
                    else
                        Target = Enemy.Base.Units.First;
                }
                else
                {
                    Target = null;
                }

                if (Index == 0 || true)
                    isMoving = !MoveTo(Target, Attack.Distance + 0.5f);
                else
                    isMoving = !MoveTo(Base.Units.List[Index - 1], 1f);

                if (Target != null && !isMoving && !isAttacking && (Index == 0 || !Base.Units.List[Index - 1].Controller.isMoving))
                    Attack.Perform();
            }
        }

        bool MoveTo(Entity target, float stoppingDistance)
        {
            return Navigator.MoveTo(target.Center, CalculatePersonalSpace(target) + stoppingDistance);
        }

        float CalculatePersonalSpace(Entity target)
        {
            return target.Bounds.extents.x + Unit.Bounds.extents.x;
        }
    }
}