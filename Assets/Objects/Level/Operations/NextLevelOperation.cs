﻿using System;
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
	public class NextLevelOperation : Operation
	{
        public static RegionsCore Levels { get { return Core.Instance.Regions; } }

        public override void Execute()
        {
            //TODO
            /*
            if(Levels.Next == null)
            {
                Debug.LogWarning("No next level");
                return;
            }

            Levels.Load(Levels.Next);
            */
        }
    }
}