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
	public class LevelFinish : Level.Module
	{
        public virtual void Process(Proponent winner)
        {
            Speed.Value = 0f;

            if (OnFinish != null)
                OnFinish();

            Data.Level.Finish(winner);

            if (winner is PlayerProponent)
            {

            }
            else
            {

            }

            Menu.End.Show(winner);
        }
    }
}