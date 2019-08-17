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
	public class LevelAges : Level.Module
	{
		[SerializeField]
        protected Age[] list;
        public Age[] List { get { return list; } }

        public int IndexOf(Age age)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i] == age)
                    return i;

            throw new Exception("Age " + age.name + " Not Registered with Level");
        }
    }
}