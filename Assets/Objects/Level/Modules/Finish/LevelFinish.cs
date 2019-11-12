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
        public virtual void Process(Proponent winner)
        {
            Level.Speed.Value = 0.2f;

            Core.Audio.Player.Music.Stop();

            if(winner is PlayerProponent)
            {
                Core.Audio.Player.SFX.PlayOneShot(SFX.Win);

                if (PlayFab.IsOnline)
                {
                    Menu.Popup.Show("Retrieving End Results");

                    PlayFab.World.FinishLevel.OnResponse += RewardResponseCallback;
                    PlayFab.World.FinishLevel.Request(Data.Level, Data.Difficulty);
                }
                else
                {
                    End();
                }
            }
            else
            {
                Core.Audio.Player.SFX.PlayOneShot(SFX.Lose);

                Menu.End.Show(winner);
            }

            if (OnProcess != null) OnProcess(winner);
        }

        PlayFabWorldFinishLevelCore.ResultData result;
        private void RewardResponseCallback(PlayFabWorldFinishLevelCore.ResultData result, PlayFabError error)
        {
            PlayFab.World.FinishLevel.OnResponse -= RewardResponseCallback;

            if(error == null)
            {
                this.result = result;

                Menu.Popup.Show("Retrieving Player Data");

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
            void Progress()
            {
                Menu.Rewards.OnFinish -= Progress;

                Menu.Rewards.Hide();
                Menu.Popup.Hide();

                Menu.End.Show(Proponents.Player);
            }

            if (result == null || result.Rewards == null || result.Rewards.Length == 0)
                Progress();
            else
            {
                Menu.Popup.Hide();

                Menu.Rewards.OnFinish += Progress;
                Menu.Rewards.Show("Level Reward", result.Rewards);
            }

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