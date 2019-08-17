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
	public class LevelMenu : Level.Module
	{
		[SerializeField]
        protected LevelEndUI end;
        public LevelEndUI End { get { return end; } }

        public class Element : UIElement, IModule<LevelMenu>
        {
            public LevelMenu Menu { get; protected set; }

            public virtual void Configure(LevelMenu data)
            {
                Menu = data;
            }

            public virtual void Init()
            {
                
            }
        }
    }
}