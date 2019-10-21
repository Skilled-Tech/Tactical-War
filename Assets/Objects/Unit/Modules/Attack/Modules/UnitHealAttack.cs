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
                if(base.CanPerform)
                {
                    if (HasTargets == false) return false;

                    return true;
                }

                return false;
            }
        }

        public bool HasTargets
        {
            get
            {
                for (int i = 1; i < Attack.Range.Value; i++)
                {
                    if (Unit.Index - i < 0) //no more allies infront to heal
                        break;

                    var target = Leader.Units[Unit.Index - i];

                    if (target.Injured) return true;
                }

                return false;
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

        public override void Connected()
        {
            base.Connected();

            for (int i = 1; i < Attack.Range.Value; i++)
            {
                if (Unit.Index - i < 0) //no more allies infront to heal
                    break;

                var target = Leader.Units[Unit.Index - i];

                if (target.Injured)
                    target.Health.Value += Attack.Power.Value;
            }
        }
    }
}