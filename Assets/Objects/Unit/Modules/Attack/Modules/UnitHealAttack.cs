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
	public class UnitHealAttack : UnitAttack
	{
        public override bool CanPerform
        {
            get
            {
                if (IsProcessing) return false;

                if (TargetsCount == 0) return false;

                return true;
            }
        }

        public int TargetsCount
        {
            get
            {
                var count = 0;

                void Process(Entity entity)
                {
                    if (entity.Injured)
                        count += 1;
                }

                ForAllTargets(Process);

                return count;
            }
        }

        protected virtual void ForAllTargets(Action<Entity> action)
        {
            for (int i = 1; i < Attack.Range.Value; i++)
            {
                if (Unit.Index - i < 0) //no more allies infront to heal
                    break;

                action(Leader.Units[Unit.Index - i]);
            }
        }
        
        public override void Init()
        {
            base.Init();
        }

        protected override void Initiate()
        {
            base.Initiate();

            Unit.Body.Animator.SetTrigger("Cast");
        }

        protected override void Connected()
        {
            base.Connected();

            ForAllTargets(Effect);
        }

        protected virtual void Effect(Entity entity)
        {
            if (entity.Injured)
                entity.Health.Value += Attack.Power.Value;
        }
    }
}