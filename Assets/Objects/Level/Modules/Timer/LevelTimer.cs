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
	public class LevelTimer : Level.Module
	{
        public float Value { get; protected set; }

        public TimeSpan TimeSpan { get; protected set; }

        protected virtual void Update()
        {
            Value += Time.deltaTime;

            TimeSpan = TimeSpan.FromSeconds(Value);
        }
	}
}