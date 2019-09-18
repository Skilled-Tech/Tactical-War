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
            Level.Speed.Value = 1f;

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
                if (result.FunctionResult == null)
                {

                }
                else
                {
                    var array = result.FunctionResult as JsonArray;

                    List<string> IDs = null;

                    if (array == null || array.Count == 0)
                        IDs = new List<string>();
                    else
                        IDs = array.ConvertAll(x => x.ToString());

                    var items = ItemRequirementData.FromIDs(IDs);

                    for (int i = 0; i < items.Count; i++)
                        Debug.Log(items[i]);
                }

                Popup.Show("Retrieving Inventory");

                Data.Level.Complete();

                PlayFab.Inventory.OnResponse += InventoryResponseCallback;
                PlayFab.Inventory.Request();
            }
        }

        void InventoryResponseCallback(PlayFabInventoryCore inventory, PlayFabError error)
        {
            PlayFab.Inventory.OnResponse -= InventoryResponseCallback;

            if (error == null)
            {
                Popup.Hide();

                Menu.End.Show(Proponents.Player);
            }
            else
            {
                ErrorCallback(error);
            }
        }

        protected virtual void ErrorCallback(PlayFabError error)
        {
            Popup.Hide();

            Menu.End.Show(Proponents.Player);
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