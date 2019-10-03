﻿using System;
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
                PlayOffline();

                PlayFab.startOffline = false;
            }
            else
            {
                PlayFab.Login.OnResponse += OnLoginResponse;
                PlayFab.Login.Perform();
            }
        }

        void PlayOffline()
        {
            PlayFab.Title.OnResponse += OnTitleResponse;
            PlayFab.Title.Request();
        }

        void OnLoginResponse(LoginResult result, PlayFabError error)
        {
            PlayFab.Login.OnResponse -= OnLoginResponse;

            if (error == null)
            {
                Popup.Show("Retrieving Title Data");

                PlayFab.Title.OnResponse += OnTitleResponse;
                PlayFab.Title.Request();
            }
            else
            {
                Popup.Show("Failed To Connect, Play Offline ?", PlayOffline, "Okay");
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
                if(PlayFab.IsLoggedIn)
                {
                    Popup.Show("Retrieving Daily Reward");

                    PlayFab.Reward.Daily.OnResponse += DailyRewardsResponseCallback;
                    PlayFab.Reward.Daily.Claim();
                }
                else
                {
                    Popup.Show("Retrieving Player Data");

                    PlayFab.Player.OnResponse += OnPlayerResponse;
                    PlayFab.Player.Retrieve();
                }
            }
            else
            {
                RaiseError(error);
            }
        }

        PlayFabLoginReward.ResultData dailyReward;
        void DailyRewardsResponseCallback(PlayFabLoginReward.ResultData result, PlayFabError error)
        {
            PlayFab.Reward.Daily.OnResponse -= DailyRewardsResponseCallback;

            if(error == null)
            {
                dailyReward = result;

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
            void Progress()
            {
                Core.UI.Rewards.OnFinish -= Progress;

                Popup.Hide();

                PlayFab.Activated = true;

                Scenes.Load(Scenes.MainMenu);
            }

            Popup.Hide();

            if(dailyReward == null || dailyReward.Items.Length == 0)
            {
                Progress();
            }
            else
            {
                var stacks = ItemStack.From(dailyReward.Items);

                var title = "Daily Reward" + Environment.NewLine;
                title += "Day " + (dailyReward.Progress + 1).ToString() + "/" + PlayFab.Reward.Daily.Max;

                Core.UI.Rewards.OnFinish += Progress;
                Core.UI.Rewards.Show(title, stacks);
            }
        }

        void RaiseError(PlayFabError error)
        {
            Debug.LogError("Login Error: " + error.GenerateErrorReport());

            Popup.Show(error.ErrorMessage);
        }
    }
}