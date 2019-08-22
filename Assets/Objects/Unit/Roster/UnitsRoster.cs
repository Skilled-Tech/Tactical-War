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
    [CreateAssetMenu(menuName = Unit.MenuPath + "Roster")]
	public class UnitsRoster : ScriptableObject
	{
		[SerializeField]
        protected UnitData[] list;
        public UnitData[] List { get { return list; } }

        public int Count { get { return list.Length; } }

        public UnitData this[int index] { get { return list[index]; } }

        public virtual void Configure()
        {
            for (int i = 0; i < list.Length; i++)
                list[i].Configure();
        }
    }
}