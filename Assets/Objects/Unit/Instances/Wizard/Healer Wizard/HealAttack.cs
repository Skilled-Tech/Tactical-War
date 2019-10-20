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
	public class HealAttack : UnitAttack.Module
	{
        public override void Init()
        {
            base.Init();
        }

        protected override void OnInitiated()
        {
            base.OnInitiated();

            if(Unit.Index == 0)
            {

            }
            else
            {
                Unit.Body.Animator.SetTrigger("Cast");
            }
        }

        protected override void OnConnected()
        {
            base.OnConnected();

            for (int i = 1; i < Attack.Range.Value; i++)
            {
                if (Unit.Index - i < 0) //no more allies infront to heal
                    break;

                var target = Leader.Units[Unit.Index - i];

                target.Health.Value += Attack.Power.Value;
            }
        }
    }
}