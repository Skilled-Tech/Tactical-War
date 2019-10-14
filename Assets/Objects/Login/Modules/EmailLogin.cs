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
	public class EmailLogin : Login.Module
	{
        public override void Execute()
        {
            base.Execute();

            PlayFab.Login.Email.Perform("Moe4Baker@gmail.com", "Password");
        }
    }
}