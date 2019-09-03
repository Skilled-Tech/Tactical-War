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

namespace Game
{
    [CreateAssetMenu(menuName = MenuPath + "Asset")]
	public class PlayFabCatalogsCore : PlayFabCore.Module
	{
        new public const string MenuPath = PlayFabCore.Module.MenuPath + "Catalogs/";

        [SerializeField]
        protected PlayFabCatalog[] elements;
        public PlayFabCatalog[] Elements { get { return elements; } }

        public virtual bool AllValid
        {
            get
            {
                for (int i = 0; i < elements.Length; i++)
                    if (elements[i].Valid == false)
                        return false;

                return true;
            }
        }

        public class Module : PlayFabCore.Module
        {
            new public const string MenuPath = PlayFabCatalogsCore.MenuPath + "Modules/";

            public PlayFabCatalogsCore Catalogs { get { return PlayFab.Catalogs; } }
        }

        public override void Configure()
        {
            base.Configure();

            for (int i = 0; i < elements.Length; i++)
                elements[i].Configure();
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
            {
                elements[i].OnRetrieved += ElementRetrieved;
                elements[i].Request();
            }
        }

        void ElementRetrieved(PlayFabCatalog element)
        {
            element.OnRetrieved -= ElementRetrieved;

            if (AllValid)
                Retrieved();
        }

        public event Action<PlayFabCatalogsCore> OnRetrieved;
        void Retrieved()
        {
            if (OnRetrieved != null) OnRetrieved(this);
        }
	}
}