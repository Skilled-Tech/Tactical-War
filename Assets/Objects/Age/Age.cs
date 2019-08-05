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
    [CreateAssetMenu]
	public class Age : ScriptableObject
	{
        [SerializeField]
        protected UnitData[] units;
        public UnitData[] Units { get { return units; } }

        [SerializeField]
        protected Currency cost;
        public Currency Cost { get { return cost; } }
    }
}