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
using PlayFab.Json;

namespace Game
{
	public class LevelFinish : Level.Module
	{
        public LevelMenu Menu { get { return Level.Menu; } }

        public LevelProponents Proponents { get { return Level.Proponents; } }

        public PlayFabCore PlayFab { get { return Core.PlayFab; } }

        public PopupUI Popup { get { return Core.UI.Popup.Instance; } }

        public delegate void ProcessDelegate(Proponent winner);
        public event ProcessDelegate OnProcess;
        public virtual void Process(Proponent winner)
        {
            Level.Speed.Value = 0.2f;

            if(winner is PlayerProponent)
            {
                if (PlayFab.IsLoggedIn)
                {
                    Popup.Show("Retrieving End Results");

                    Request(Data.Level);
                }
                else
                {
                    Level.Speed.Value = 0f;

                    Menu.End.Show(winner);
                }
            }
            else
            {
                Level.Speed.Value = 0f;

                Menu.End.Show(winner);
            }

            if (OnProcess != null) OnProcess(winner);
        }

        protected virtual void Request(LevelCore level)
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "FinishLevel",
                FunctionParameter = new ParametersData(level.Region.name, level.Index),
                GeneratePlayStreamEvent = true,
            };

            PlayFabClientAPI.ExecuteCloudScript(request, ResultCallback, ErrorCallback);
        }

        ExecuteCloudScriptResult cloudscriptResult;
        protected virtual void ResultCallback(ExecuteCloudScriptResult result)
        {
            if(result.FunctionResult == null)
            {
                Debug.Log(result.FunctionResult);

                for (int i = 0; i < result.Logs.Count; i++)
                    Debug.LogError(result.Logs[i].Data);
            }
            else
            {
                cloudscriptResult = result;

                Popup.Show("Retrieving Inventory");

                PlayFab.Inventory.OnResponse += InventoryResponseCallback;
                PlayFab.Inventory.Request();
            }
        }

        void InventoryResponseCallback(PlayFabInventoryCore inventory, PlayFabError error)
        {
            PlayFab.Inventory.OnResponse -= InventoryResponseCallback;

            if (error == null)
            {
                Popup.Show("Retrieving Player Profile");

                PlayFab.Player.OnResponse += OnPlayFabPlayerResponse;
                PlayFab.Player.Retrieve();
            }
            else
            {
                ErrorCallback(error);
            }
        }

        void OnPlayFabPlayerResponse(PlayFabPlayerCore result, PlayFabError error)
        {
            PlayFab.Player.OnResponse -= OnPlayFabPlayerResponse;

            if (error == null)
            {
                var array = cloudscriptResult.FunctionResult as JsonArray;

                var rewards = GetRewards(array);

                if (rewards.Count == 0)
                {
                    ShowDialog();
                }
                else
                {
                    Popup.Hide();

                    Menu.Rewards.OnFinish += OnRewardsProcessed;
                    Menu.Rewards.Show(rewards);
                }
            }
            else
            {

            }
        }

        IList<ItemRequirementData> GetRewards(JsonArray jArray)
        {
            List<string> IDs = null;

            if (jArray == null || jArray.Count == 0)
                IDs = new List<string>();
            else
                IDs = jArray.ConvertAll(x => x.ToString());

            var items = ItemRequirementData.FromIDs(IDs);

            return items;
        }

        void OnRewardsProcessed()
        {
            Menu.Rewards.Hide();

            ShowDialog();
        }
        
        void ShowDialog()
        {
            Popup.Hide();

            Menu.End.Show(Proponents.Player);
        }

        protected virtual void ErrorCallback(PlayFabError error)
        {
            void Continue()
            {
                Popup.Hide();

                Menu.End.Show(Proponents.Player);
            }

            Popup.Show(error.ErrorMessage, Continue, "Continue");
        }
        
        struct ParametersData
        {
            public string region;
            public int level;

            public ParametersData(string region, int level)
            {
                this.region = region;
                this.level = level;
            }
        }
    }
}