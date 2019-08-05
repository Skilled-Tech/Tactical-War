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
	public class LevelProponents : Reference<Level>
	{
        [SerializeField]
        protected PlayerProponent player;
        public PlayerProponent Player { get { return player; } }

        [SerializeField]
        protected Proponent enemy;
        public Proponent Enemy { get { return enemy; } }

        protected virtual void Awake()
        {

        }
    }
}