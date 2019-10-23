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
using PlayFab;

namespace Game
{
	public class EmailLogin : Login.Controller
    {
        [SerializeField]
        protected TMP_InputField address;
        public TMP_InputField Address { get { return address; } }

        [SerializeField]
        protected TMP_InputField password;
        public TMP_InputField Password { get { return password; } }

        [SerializeField]
        protected Button confirm;
        public Button Confirm { get { return confirm; } }

        public override bool Accessible => true;

        public override bool Available => true;

        public override void Configure(Login reference)
        {
            base.Configure(reference);

            confirm.onClick.AddListener(Perform);

            address.onValueChanged.AddListener(EmailChangeCallback);
            password.onValueChanged.AddListener(PasswordChangeCallback);

            UpdateState();
        }

        void PasswordChangeCallback(string value)
        {
            UpdateState();
        }

        void EmailChangeCallback(string value)
        {
            UpdateState();
        }

        protected override void UpdateState()
        {
            base.UpdateState();

            bool IsValid()
            {
                if (string.IsNullOrEmpty(address.text))
                    return false;

                if (string.IsNullOrEmpty(password.text))
                    return false;

                return true;
            }

            confirm.interactable = IsValid();
        }

        public override void Perform()
        {
            base.Perform();

            Popup.Show("Logging In");

            PlayFab.Login.Email.Perform(address.text, password.text);
        }

        protected override void ErrorProcedure(PlayFabError error)
        {
            Popup.Show(error.ErrorMessage, Popup.Hide, "Close");
        }
    }
}