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
	public class UnitEffectAttack : UnitAttack.Module
	{
        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        List<Entity> targets;

        public override void Configure(UnitAttack data)
        {
            base.Configure(data);

            targets = new List<Entity>();
        }

        protected override void AttackConnected()
        {
            base.AttackConnected();

            AquireTargets();

            Process();
        }

        protected virtual void AquireTargets()
        {
            targets.Clear();

            if (Enemy.Base.Units.Count == 0)
            {
                targets.Add(Enemy.Base);
            }
            else
            {
                var count = Attack.Range;

                if (Unit.Index > 1) count -= (uint)Unit.Index;

                for (int i = 0; i < count; i++)
                {
                    if (i >= Enemy.Units.Count)
                        break;

                    if(i > 0)
                    {
                        if (DistanceBetween(Enemy.Units[i], Enemy.Units[i - 1]) > Attack.Distance * 1.5f)
                            break;
                    }

                    targets.Add(Enemy.Units[i]);
                }
            }
        }
        protected virtual float DistanceBetween(Entity one, Entity two)
        {
            return Vector3.Distance(one.Center, two.Center);
        }

        protected virtual void Process()
        {
            for (int i = 0; i < targets.Count; i++)
                Perform(targets[i]);
        }

        protected virtual void Perform(Entity target)
        {
            Vector3 GetPosition(Entity entity) => target.Center + Vector3.down * target.Bounds.extents.y;

            var instance = Instantiate(prefab, GetPosition(target), prefab.transform.rotation);

            DoDamage(target);
        }
    }
}