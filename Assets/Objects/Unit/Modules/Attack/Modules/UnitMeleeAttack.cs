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
	public class UnitMeleeAttack : UnitAttack.Module
	{
        public virtual Entity Target
        {
            get
            {
                if (Enemy.Base.Units.Count == 0)
                    return Leader.Enemy.Base;

                return Enemy.Base.Units.First;
            }
        }

        protected override void AttackConnected()
        {
            base.AttackConnected();

            if (Target != null) DoDamage(Target);
        }
    }
}