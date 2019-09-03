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

using PlayFab;
using PlayFab.ClientModels;

namespace Game
{
	public class Login : MonoBehaviour
	{
		[SerializeField]
        protected PopupUI popup;
        public PopupUI Popup { get { return popup; } }

        public Core Core { get { return Core.Instance; } }

        public ScenesCore Scenes { get { return Core.Scenes; } }

        public PlayFabCore PlayFab { get { return Core.PlayFab; } }

        void Start()
        {
            popup.Show("Logging In", null, null);

            PlayFab.LoginEvent += OnLogin;
            PlayFab.Login();
        }

        void OnLogin(LoginResult result)
        {
            PlayFab.LoginEvent -= OnLogin;

            Debug.Log("Login Successful");

            Popup.Text = "Retrieving Catalogs";

            PlayFab.Catalogs.OnRetrieved += OnCatalogsRetrieved;
            PlayFab.Catalogs.Request();
        }

        void OnCatalogsRetrieved(PlayFabCatalogsCore catalogs)
        {
            PlayFab.Catalogs.OnRetrieved -= OnCatalogsRetrieved;

            Popup.Text = "Retrieving Inventory";

            PlayFab.Inventory.OnRetrieved += OnInventoryRetrieved;
            PlayFab.Inventory.Request();
        }

        void OnInventoryRetrieved(PlayFabInventoryCore inventory)
        {
            PlayFab.Inventory.OnRetrieved -= OnInventoryRetrieved;

            Finish();
        }

        void Finish()
        {
            Scenes.Load(Scenes.MainMenu);
        }
    }
}