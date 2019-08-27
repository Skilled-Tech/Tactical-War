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
using Newtonsoft.Json.Linq;

namespace Game
{
    [CreateAssetMenu(menuName = MenuPath + "Units")]
	public class UnitsCore : Core.Module
	{
        [SerializeField]
        protected UnitTemplate[] list;
        public UnitTemplate[] List { get { return list; } }

        public UnitTemplate this[int index] { get { return list[index]; } }

        public int Count { get { return list.Length; } }

        public virtual UnitTemplate Find(string name)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].name == name)
                    return list[i];

            return null;
        }
    }
}