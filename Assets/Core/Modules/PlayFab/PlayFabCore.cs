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
    [CreateAssetMenu(menuName = MenuPath + "Asset")]
	public class PlayFabCore : Core.Module
	{
        new public const string MenuPath = Core.Module.MenuPath + "PlayFab/";

        [SerializeField]
        protected PlayFabRequestsCore requests;
        public PlayFabRequestsCore Requests { get { return requests; } }

        [SerializeField]
        protected PlayFabCatalogsCore catalogs;
        public PlayFabCatalogsCore Catalogs { get { return catalogs; } }

        public class Module : Core.Module
        {
            public PlayFabCore PlayFab { get { return Core.PlayFab; } }

            new public const string MenuPath = PlayFabCore.MenuPath + "Modules/";
        }

        public override void Configure()
        {
            base.Configure();

            requests.Configure();

            catalogs.Configure();

            Requests.EmailLogin.OnResult += OnLogin;
        }

        public override void Init()
        {
            base.Init();

            catalogs.Init();

            requests.Init();
        }

        public virtual void Login()
        {
            Requests.EmailLogin.Request("Moe4Baker@gmail.com", "Password");
        }

        public event PlayFabRequestsCore.EmailLoginHandler.ResaultDelegate LoginEvent;
        void OnLogin(LoginResult result)
        {
            if (LoginEvent != null) LoginEvent(result);
        }

        public virtual void OnError(PlayFabError error)
        {

        }
    }
}