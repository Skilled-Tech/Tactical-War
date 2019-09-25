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
    [CreateAssetMenu(menuName = MenuPath + "Paralysis")]
	public class ParalysisStatusEffect : StatusEffectType
	{
        [SerializeField]
        [Tooltip("Delay that the entity experiences when having this effect applied to them untill they can function again")]
        protected float delay = 0.75f;
        public float Delay { get { return delay; } }

        public override void Apply(StatusEffectInstance effect)
        {

        }
    }
}