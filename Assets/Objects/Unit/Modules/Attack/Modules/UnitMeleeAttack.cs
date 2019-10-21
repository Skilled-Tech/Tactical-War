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
	public class UnitMeleeAttack : UnitAttack
	{
        protected override void Initiate()
        {
            base.Initiate();

            Unit.Body.CharacterAnimation.Attack();
        }

        public override void Connected()
        {
            base.Connected();

            if (Target != null) DoDamage(Target);
        }
    }
}