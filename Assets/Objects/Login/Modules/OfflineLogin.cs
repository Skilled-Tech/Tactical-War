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
    public class OfflineLogin : Login.Controller
    {
        public override bool IsValid
        {
            get
            {
                if (Core.Prefs.NeedOnlineLogin.Value) return false;

                return true;
            }
        }

        public override void Show()
        {
            base.Show();

            Popup.Show("Logging In");

            Login.Offline();
        }
    }
}