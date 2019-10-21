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
    public class UnitEffectAttack : UnitAttack
    {
        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        [SerializeField]
        protected float intervals = 0.25f;
        public float Intervals { get { return intervals; } }

        List<Entity> targets;

        public override void Configure(Unit data)
        {
            base.Configure(data);

            targets = new List<Entity>();
        }

        protected override void Initiate()
        {
            base.Initiate();

            Unit.Body.Animator.SetTrigger("Cast");
        }

        public override void Connected()
        {
            base.Connected();

            AquireTargets();

            Process();
        }

        protected virtual void AquireTargets()
        {
            float DistanceBetween(Entity one, Entity two)
            {
                return one.DistanceTo(two);
            }

            targets.Clear();

            if (Enemy.Base.Units.Count == 0)
            {
                targets.Add(Enemy.Base);
            }
            else
            {
                var count = Attack.Range.Value - Unit.Index - 1;

                for (int i = 0; i < count; i++)
                {
                    if (i >= Enemy.Units.Count)
                        break;

                    if (i == 0)
                    {
                        if (DistanceBetween(Enemy.Units[i], Unit) > Attack.Distance * 3f) break;
                    }
                    else
                    {
                        if (DistanceBetween(Enemy.Units[i], Enemy.Units[i - 1]) > Attack.Distance * 1.5f) break;
                    }

                    targets.Add(Enemy.Units[i]);
                }
            }
        }

        protected virtual void Process()
        {
            StartCoroutine(Procedure());
        }

        IEnumerator Procedure()
        {
            for (int i = 0; i < targets.Count; i++)
            {
                Effect(targets[i]);

                yield return new WaitForSeconds(intervals);
            }
        }

        protected virtual void Effect(Entity target)
        {
            Vector3 GetPosition(Entity entity) => target.Center + Vector3.down * target.Bounds.extents.y;

            var instance = Instantiate(prefab, GetPosition(target), prefab.transform.rotation);

            DoDamage(target);
        }
    }
}