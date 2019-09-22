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
using PlayFab.SharedModels;

using Newtonsoft.Json;

namespace Game
{
    [Serializable]
	public class PlayFabRewardCore : PlayFabCore.Module
	{
        [SerializeField]
        protected PlayFabLoginReward daily;
        public PlayFabLoginReward Daily { get { return daily; } }

        [SerializeField]
        protected PlayFabLevelRewardCore level;
        public PlayFabLevelRewardCore Level { get { return level; } }

        public override void Configure()
        {
            base.Configure();

            Register(daily);
            Register(level);
        }

        [Serializable]
        public class Module : PlayFabCore.Module
        {
            public PlayFabRewardCore Reward { get { return PlayFab.Reward;} }
        }
	}
}