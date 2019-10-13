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
using PlayFab.SharedModels;

using Newtonsoft.Json;

namespace Game
{
    [Serializable]
    public class PlayFabWorldCore : PlayFabCore.Property
    {
        [SerializeField]
        protected PlayFabWorldFinishLevelCore finishLevel;
        public PlayFabWorldFinishLevelCore FinishLevel { get { return finishLevel; } }

        [Serializable]
        public class Module : PlayFabCore.Property
        {
            public PlayFabWorldCore World { get { return PlayFab.World; } }
        }
    }
}