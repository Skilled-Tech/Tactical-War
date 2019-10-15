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

using TMPro;

namespace Game
{
	public class EmailLogin : Login.Module
	{
        [SerializeField]
        protected TMP_InputField email;
        public TMP_InputField Email { get { return email; } }

        [SerializeField]
        protected TMP_InputField password;
        public TMP_InputField Password { get { return password; } }

        [SerializeField]
        protected Button confirm;
        public Button Confirm { get { return confirm; } }

        public override void Configure(Login reference)
        {
            base.Configure(reference);

            confirm.onClick.AddListener(Action);

            email.onValueChanged.AddListener(EmailChangeCallback);
            password.onValueChanged.AddListener(PasswordChangeCallback);

            UpdateState();
        }

        void PasswordChangeCallback(string arg0)
        {
            UpdateState();
        }

        void EmailChangeCallback(string arg0)
        {
            UpdateState();
        }

        protected virtual void UpdateState()
        {
            bool IsValid()
            {
                if (string.IsNullOrEmpty(email.text))
                    return false;

                if (string.IsNullOrEmpty(password.text))
                    return false;

                return true;
            }

            confirm.interactable = IsValid();
        }

        public virtual void Action()
        {
            Popup.Show("Logging In");

            PlayFab.Login.Email.Perform(email.text, password.text);
        }
    }
}