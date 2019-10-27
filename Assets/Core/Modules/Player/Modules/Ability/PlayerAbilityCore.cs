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
    [Serializable]
	public class PlayerAbilityCore : PlayerCore.Module
	{
        [SerializeField]
        protected PlayerAbilitySelectionCore selection;
        public PlayerAbilitySelectionCore Selection { get { return selection; } }

        public class Module : PlayerCore.Module
        {
            public PlayerUnitsCore Units { get { return Player.Units; } }
        }

        public override void Configure()
        {
            base.Configure();

            Register(selection);
        }
    }
}