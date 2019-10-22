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
	public class GuestLogin : Login.Controller
    {
        public override bool IsValid => true;

        public override void Show()
        {
            base.Show();

            Popup.Show("Logging In");

            if (Application.isEditor)
            {
                PlayFab.Login.Email.Perform("Moe4Baker@gmail.com", "Password");
            }
            else if(Application.platform == RuntimePlatform.Android)
            {
                PlayFab.Login.AndroidDeviceID.Perform();
            }
            else if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}