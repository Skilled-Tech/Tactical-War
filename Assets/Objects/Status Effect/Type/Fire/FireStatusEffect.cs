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
    [CreateAssetMenu(menuName = MenuPath + "Fire")]
	public class FireStatusEffect : StatusEffectType
	{
        public override void Apply(StatusEffectInstance effect)
        {
            effect.Affector.DoDamage(effect.Data.Potency, Damage.Method.Ranged, effect.Target);
        }
    }
}