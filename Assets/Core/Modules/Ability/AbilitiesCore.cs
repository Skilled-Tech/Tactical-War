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
	public class AbilitiesCore : Core.Property
	{
		[SerializeField]
        protected Ability[] list;
        public Ability[] List { get { return list; } }

        public Ability this[int index] => list[index];

        public int Count => list.Length;
    }
}