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
            Debug.Log(result.FunctionResult);

            Popup.Hide();

            Data.Level.Complete();

            Menu.End.Show(Proponents.Player);
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