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

namespace Game
{
    [CreateAssetMenu(menuName = MenuPath + "Asset")]
	public class PlayFabCatalogsCore : PlayFabCore.Module
	{
        new public const string MenuPath = PlayFabCore.Module.MenuPath + "Catalogs/";

        [SerializeField]
        protected PlayFabCatalog[] elements;
        public PlayFabCatalog[] Elements { get { return elements; } }

        public class Module : PlayFabCore.Module
        {
            new public const string MenuPath = PlayFabCatalogsCore.MenuPath + "Modules/";

            public PlayFabCatalogsCore Catalogs { get { return PlayFab.Catalogs; } }
        }

        public override void Configure()
        {
            base.Configure();

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].Configure();

                elements[i].OnRetrieved += OnElementRetrieved;
                elements[i].OnResponse += OnElementResponse;
            }
        }

        public override void Init()
        {
            base.Init();

            for (int i = 0; i < elements.Length; i++)
                elements[i].Init();
        }

        public virtual void Request()
        {
            for (int i = 0; i < elements.Length; i++)
                elements[i].Request();
        }

        public event PlayFabRequestsUtility.ResaultDelegate<PlayFabCatalogsCore> OnResult;
        void OnElementRetrieved(PlayFabCatalog result)
        {
            if (elements.All(x => x.Valid))
                if (OnResult != null) OnResult(this);
        }

        public event PlayFabRequestsUtility.ResponseDelegate<PlayFabCatalogsCore> OnResponse;
        void OnElementResponse(PlayFabCatalog result, PlayFabError error)
        {
            if(error == null)
            {
                if (elements.All(x => x.Valid))
                    if (OnResponse != null) OnResponse(this, error);
            }
            else
            {
                if (OnResponse != null) OnResponse(this, error);
            }
        }
    }
}