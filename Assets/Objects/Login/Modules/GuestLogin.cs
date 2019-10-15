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
	public class GuestLogin : Login.Module
    {
        public override void Show()
        {
            base.Show();

            Popup.Show("Logging In");

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    PlayFab.Login.AndroidDeviceID.Perform();
                    break;

                case RuntimePlatform.IPhonePlayer:
                    throw new NotImplementedException();

                default:
                    PlayFab.Login.Email.Perform("Moe4Baker@gmail.com", "Password");
                    break;
            }
        }
    }
}