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

using UnityEngine.Advertisements;

namespace Game
{
	public class LevelFinish : Level.Module
	{
        [SerializeField]
        protected SFXData _SFX;
        public SFXData SFX { get { return _SFX; } }
        [Serializable]
        public class SFXData
        {
            [SerializeField]
            protected SFXProperty win;
            public SFXProperty Win { get { return win; } }

            [SerializeField]
            protected SFXProperty lose;
            public SFXProperty Lose { get { return lose; } }
        }

        public LevelMenu Menu { get { return Level.Menu; } }

        public LevelProponents Proponents { get { return Level.Proponents; } }

        public PlayFabCore PlayFab { get { return Core.PlayFab; } }

        public delegate void ProcessDelegate(Proponent winner);
        public event ProcessDelegate OnProcess;
        Proponent winner = null;
        public virtual void Process(Proponent winner)
        {
            Level.Speed.Value = 0.2f;

            Core.Audio.Player.Music.Stop();

            this.winner = winner;

            if(winner is PlayerProponent)
            {
                Core.Audio.Player.SFX.PlayOneShot(SFX.Win);

                if (PlayFab.IsOnline)
                    Request();
                else
                {
                    End();
                }
            }
            else
            {
                Core.Audio.Player.SFX.PlayOneShot(SFX.Lose);

                End();
            }

            if (OnProcess != null) OnProcess(winner);
        }

        public PlayFabWorldFinishLevelCore.ResultData result;
        public List<ItemTemplate> rewards = new List<ItemTemplate>();
        protected virtual void Request()
        {
            Menu.Popup.Show(Core.Localization.Phrases.Get("Retrieving End Results"));

            PlayFab.World.FinishLevel.OnResponse += Callback;
            PlayFab.World.FinishLevel.Request(Data.Level, Data.Difficulty, Level.Timer.Value);

            void Callback(PlayFabWorldFinishLevelCore.ResultData result, PlayFabError error)
            {
                PlayFab.World.FinishLevel.OnResponse -= Callback;

                this.result = result;

                if (error == null)
                {
                    rewards.AddRange(result.Rewards);

                    if(Data.Level.Completed == false) //This is going to be the first time we complete this level
                    {
                        rewards.AddRange(Data.Level.Unlocks);
                    }

                    RetrivePlayerData();
                }
                else
                {
                    RaiseError(error);
                }
            }
        }
        
        void RetrivePlayerData()
        {
            Menu.Popup.Show(Core.Localization.Phrases.Get("Retrieving Player Data"));

            PlayFab.Player.OnResponse += Callback;
            PlayFab.Player.Retrieve();

            void Callback(PlayFabPlayerCore result, PlayFabError error)
            {
                PlayFab.Player.OnResponse -= Callback;

                if (error == null)
                {
                    ShowRewards();
                }
                else
                {

                }
            }
        }
        
        void ShowRewards()
        {
            if (rewards.Count == 0)
            {
                End();
            }
            else
            {
                Menu.Popup.Hide();

                Menu.Rewards.OnFinish += Callback;
                Menu.Rewards.Show(Core.Localization.Phrases.Get("Level Reward"), rewards);

                void Callback()
                {
                    Menu.Rewards.OnFinish -= Callback;

                    End();
                }
            }
        }

        void End()
        {
            Menu.Rewards.Hide();
            Menu.Popup.Hide();

            Menu.End.Show(winner);

            if (Core.Ads.Active && Advertisement.IsReady())
            {
                Advertisement.Show();
            }

            if (result == null)
                Menu.End.Stars.Hide();
            else
                Menu.End.Stars.Show(result.Stars);

            Level.Speed.Value = 0f;
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