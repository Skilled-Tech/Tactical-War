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

using GooglePlayGames;

namespace Game
{
	public class LogoutOperation : Operation
	{
        public ScenesCore Scenes => Core.Instance.Scenes;

        public override void Execute()
        {
            PlayGamesPlatform.Instance.SignOut();

            Scenes.Load.One(Scenes.Login);
        }
    }
}