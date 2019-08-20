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
        public Proponent Enemy { get { return Leader.Enemey; } }

        public bool IsAlive { get { return Unit.IsAlive; } }

        public Level Level { get { return Level.Instance; } }
        public LevelProponents Proponents { get { return Level.Proponents; } }

        public Entity Target { get; protected set; }

        public bool isMoving { get; protected set; }
        public bool isAttacking { get { return Attack.IsProcessing; } }

        public float Spacing
        {
            get
            {
                return 1f;
            }
        }

        void Update()
        {
            if(isAttacking)
            {

            }
            else
            {
                if (Index < Attack.Range)
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

                if (Index == 0)
                    isMoving = !Navigator.MoveTo(Target.transform.position, Attack.Distance + CalculatePersonalSpace(Target));
                else
                    isMoving = !MoveTo(Base.Units.List[Index - 1], Vector3.left, Spacing, 0f);

                if (Target != null && !isMoving && !isAttacking)
                    Attack.Do(Target);
            }
        }

        bool MoveTo(Entity target, Vector3 direction, float space, float stoppingDistance)
        {
            var position = target.transform.position;

            position += target.transform.right * direction.x * (CalculatePersonalSpace(target) + space);

            return Navigator.MoveTo(position, stoppingDistance);
        }

        float CalculatePersonalSpace(Entity target)
        {
            if (target is Base)
                return target.Bounds.extents.x;

            return 1f;
        }

        private void OnDrawGizmosSelected()
        {
            if(Target != null && Index == 0)
            {
                var position = Target.transform.position;

                position += Target.transform.right * Vector3.right.x * (Target.Bounds.extents.x + Unit.Bounds.extents.x + Spacing);

                Gizmos.DrawSphere(position, 0.4f);
            }
        }
    }
}