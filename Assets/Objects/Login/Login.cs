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

            PlayFab.Login.OnResponse += OnLoginResponse;
            PlayFab.Login.Perform();
        }

        void OnLoginResponse(PlayFabCore.LoginCore result, PlayFabError error)
        {
            if(error == null)
            {
                PlayFab.Login.OnResponse -= OnLoginResponse;

                Popup.Text = "Retrieving Title Data";

                PlayFab.Title.OnResponse += OnTitleResponse;
                PlayFab.Title.Request();
            }
            else
            {
                RaiseError(error);
            }
        }

        void OnTitleResponse(PlayFabCore.TitleCore result, PlayFabError error)
        {
            PlayFab.Title.OnResponse -= OnTitleResponse;

            if(error == null)
            {
                Popup.Show("Retrieving Catalog");

                PlayFab.Catalogs.OnResponse += OnCatalogsResponse;
                PlayFab.Catalogs.Request();
            }
            else
            {
                RaiseError(error);
            }
        }

        void OnCatalogsResponse(PlayFabCore.CatalogsCore catalogs, PlayFabError error)
        {
            PlayFab.Catalogs.OnResponse -= OnCatalogsResponse;

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

        void OnInventoryResponse(PlayFabCore.InventoryCore inventory, PlayFabError error)
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
            Debug.Log("Finished");
            return;

            Scenes.Load(Scenes.MainMenu);
        }

        void Update()
        {
            if(Core.Instance.PlayFab.Inventory.Items != null)
            {
                var item = Core.Instance.PlayFab.Inventory.Items[0];

                if (Input.GetKeyDown(KeyCode.D))
                {
                    Upgrade(item.ItemInstanceId, "Damage");
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Upgrade(item.ItemInstanceId, "Defense");
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Upgrade(item.ItemInstanceId, "Range");
                }
            }
        }

        void Upgrade(string itemInstanceID, string type)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "UpgradeItem",
                FunctionParameter =
                new
                {
                    ItemInstanceId = itemInstanceID,
                    UpgradeType = type,
                },
                GeneratePlayStreamEvent = true,
            };

            PlayFabClientAPI.ExecuteCloudScript(request, UpgradeItemCallback, RaiseError);
        }

        void UpgradeItemCallback(ExecuteCloudScriptResult result)
        {
            Debug.Log(result.FunctionResult);
        }

        void RaiseError(PlayFabError error)
        {
            Debug.LogError("Login Error: " + error.GenerateErrorReport());

            Popup.Show(error.ErrorMessage);
        }
    }
}