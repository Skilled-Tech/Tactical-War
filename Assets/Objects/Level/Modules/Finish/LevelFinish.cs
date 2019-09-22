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

        public delegate void ProcessDelegate(Proponent winner);
        public event ProcessDelegate OnProcess;
        public virtual void Process(Proponent winner)
        {
            Level.Speed.Value = 0.2f;

            if(winner is PlayerProponent)
            {
                if (PlayFab.IsLoggedIn)
                {
                    Menu.Popup.Show("Retrieving End Results");

                    PlayFab.LevelReward.OnResponse += LevelRewardsResponseCallback;
                    PlayFab.LevelReward.Retrieve();
                }
                else
                {
                    End();
                }
            }
            else
            {
                Level.Speed.Value = 0f;

                Menu.End.Show(winner);
            }

            if (OnProcess != null) OnProcess(winner);
        }

        private void LevelRewardsResponseCallback(IList<ItemRequirementData> result, PlayFabError error)
        {
            void Progress()
            {
                Menu.Rewards.OnFinish -= Progress;

                PlayFab.Player.Inventory.OnResponse += InventoryResponseCallback;
                PlayFab.Player.Inventory.Request();
            }

            if(error == null)
            {
                if (result == null || result.Count == 0)
                    Progress();
                else
                {
                    Menu.Rewards.OnFinish += Progress;
                    Menu.Rewards.Show(result);
                }
            }
            else
            {
                RaiseError(error);
            }
        }

        void InventoryResponseCallback(PlayFabPlayerInventoryCore inventory, PlayFabError error)
        {
            PlayFab.Player.Inventory.OnResponse -= InventoryResponseCallback;

            if (error == null)
            {
                Menu.Popup.Show("Retrieving Player Profile");

                PlayFab.Player.OnResponse += OnPlayFabPlayerResponse;
                PlayFab.Player.Retrieve();
            }
            else
            {
                RaiseError(error);
            }
        }

        void OnPlayFabPlayerResponse(PlayFabPlayerCore result, PlayFabError error)
        {
            PlayFab.Player.OnResponse -= OnPlayFabPlayerResponse;

            if (error == null)
            {
                End();
            }
            else
            {

            }
        }

        void End()
        {
            Level.Speed.Value = 0f;

            Menu.Rewards.Hide();
            Menu.Popup.Hide();

            Menu.End.Show(Proponents.Player);
        }

        protected virtual void RaiseError(PlayFabError error)
        {
            void Continue()
            {
                Menu.Popup.Hide();

                End();
            }

            Menu.Popup.Show(error.ErrorMessage, Continue, "Continue");
        }
    }
}