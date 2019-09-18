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
        public PopupUI Popup { get { return Core.UI.Popup.Instance; } }

        public Core Core { get { return Core.Instance; } }

        public ScenesCore Scenes { get { return Core.Scenes; } }

        public PlayerCore Player { get { return Core.Player; } }

        public PlayFabCore PlayFab { get { return Core.PlayFab; } }

        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            Popup.Show("Logging In", null, null);

            if (Input.GetKey(KeyCode.X) && Application.isEditor)
            {
                OnLoginResponse(null, new PlayFabError());
            }
            else
            {
                PlayFab.Login.OnResponse += OnLoginResponse;
                PlayFab.Login.Perform();
            }
        }

        void OnLoginResponse(LoginResult result, PlayFabError error)
        {
            PlayFab.Login.OnResponse -= OnLoginResponse;

            void Progress()
            {
                Popup.Text = "Retrieving Title Data";

                PlayFab.Title.OnResponse += OnTitleResponse;
                PlayFab.Title.Request();
            }

            if (error == null)
            {
                Progress();
            }
            else
            {
                Popup.Show("Failed To Connect, Play Offline ?", Progress, "Ok");
            }
        }

        void OnTitleResponse(PlayFabTitleCore data, PlayFabError error)
        {
            PlayFab.Title.OnResponse -= OnTitleResponse;

            if (error == null)
            {
                Popup.Show("Retrieving Player Data");

                PlayFab.Player.Retrieve();
                PlayFab.Player.OnResponse += OnPlayerDataResponse;
            }
            else
            {
                RaiseError(error);
            }
        }

        void OnPlayerDataResponse(PlayFabPlayerCore result, PlayFabError error)
        {
            PlayFab.Player.OnResponse -= OnPlayerDataResponse;

            if (error == null)
            {
                Popup.Show("Retrieving Catalog");

                PlayFab.Catalog.OnResponse += OnCatalogResponse;
                PlayFab.Catalog.Request();
            }
            else
            {
                RaiseError(error);
            }
        }

        void OnCatalogResponse(PlayFabCatalogCore catalog, PlayFabError error)
        {
            PlayFab.Catalog.OnResponse -= OnCatalogResponse;

            if (error == null)
            {
                Popup.Text = "Retrieving Inventory";

                PlayFab.Inventory.OnResponse += OnInventoryResponse;
                PlayFab.Inventory.Request();
            }
            else
            {
                RaiseError(error);
            }
        }

        void OnInventoryResponse(PlayFabInventoryCore inventory, PlayFabError error)
        {
            PlayFab.Inventory.OnResponse -= OnInventoryResponse;

            if (error == null)
            {
                Finish();
            }
            else
            {
                RaiseError(error);
            }
        }

        void Finish()
        {
            Popup.Hide();

            PlayFab.Activated = true;

            Scenes.Load(Scenes.MainMenu);
        }

        void RaiseError(PlayFabError error)
        {
            Debug.LogError("Login Error: " + error.GenerateErrorReport());

            Popup.Show(error.ErrorMessage);
        }
    }
}