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
using PlayFab.Json;
using PlayFab.ClientModels;

namespace Game
{
    public class Login : MonoBehaviour
    {
        public PopupUI Popup { get { return Core.UI.Popup; } }

        public Core Core { get { return Core.Instance; } }

        public ScenesCore Scenes { get { return Core.Scenes; } }

        public PlayerCore Player { get { return Core.Player; } }

        public PlayFabCore PlayFab { get { return Core.PlayFab; } }

        void Start()
        {
            Popup.Show("Logging In", null, null);

            if (PlayFab.startOffline && Application.isEditor)
            {
                Progress();

                PlayFab.startOffline = false;
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

            if (error == null)
            {
                Progress();
            }
            else
            {
                Popup.Show("Failed To Connect, Play Offline ?", Progress, "Ok");
            }
        }

        void Progress()
        {
            PlayFab.DailyReward.OnResponse += DailyRewardsResponseCallback;
            PlayFab.DailyReward.Perform();
        }

        void DailyRewardsResponseCallback(PlayFabDailyRewardCore.ResultData result, PlayFabError error)
        {
            void Progress()
            {
                Core.UI.Rewards.Hide();
                Popup.Show("Retrieving Title Data");

                PlayFab.Title.OnResponse += OnTitleResponse;
                PlayFab.Title.Request();
            }

            if(error == null)
            {
                Debug.Log(result.Progress);

                foreach (var item in result.Items)
                    Debug.Log(item.ID);

                if (result.Items.Length == 0)
                {
                    Progress();
                }
                else
                {
                    Popup.Hide();

                    Core.UI.Rewards.OnFinish += Progress;
                    Core.UI.Rewards.Show(result.Items);
                }
            }
            else
            {

            }
        }

        void OnTitleResponse(PlayFabTitleCore data, PlayFabError error)
        {
            PlayFab.Title.OnResponse -= OnTitleResponse;

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
                Popup.Show("Retrieving Player Data");

                PlayFab.Player.OnResponse += OnPlayerResponse;
                PlayFab.Player.Retrieve();
            }
            else
            {
                RaiseError(error);
            }
        }

        void OnPlayerResponse(PlayFabPlayerCore result, PlayFabError error)
        {
            PlayFab.Player.OnResponse -= OnPlayerResponse;

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