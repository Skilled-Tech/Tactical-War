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
	public class ProponentAge : Proponent.Module
	{
        public Age Value { get; protected set; }
        public int Index { get { return Level.Ages.IndexOf(Value); } }

        public event Action<Age> OnValueChanged;
        public virtual void Set(Age age)
        {
            Value = age;

            if (OnValueChanged != null) OnValueChanged(Value);
        }
	}
}