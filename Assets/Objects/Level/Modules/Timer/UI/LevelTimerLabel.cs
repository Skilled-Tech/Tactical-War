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

using TMPro;

namespace Game
{
    [RequireComponent(typeof(TMP_Text))]
	public class LevelTimerLabel : UIElement
	{
        public TMP_Text Label { get; protected set; }

        public Level Level { get { return Level.Instance; } }

        protected virtual void Awake()
        {
            Label = GetComponent<TMP_Text>();
        }

		protected virtual void Update()
        {
            Label.text = Level.Timer.TimeSpan.ToString(@"mm\:ss");
        }
	}
}