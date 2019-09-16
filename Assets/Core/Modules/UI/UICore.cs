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
    [Serializable]
	public class UICore : Core.Module
	{
        [SerializeField]
        protected PopupUICore popup;
        public PopupUICore Popup { get { return popup; } }

        [Serializable]
        public class Module : Core.Module
        {
            public UICore UI { get { return Core.UI; } }
        }

        public override void Configure()
        {
            base.Configure();

            popup.Configure();
        }

        public override void Init()
        {
            base.Init();

            popup.Init();
        }
    }

    [Serializable]
    public class PopupUICore : UICore.Module
    {
        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        public PopupUI Instance { get; protected set; }

        public override void Configure()
        {
            base.Configure();

            Instance = Create();
        }

        PopupUI Create()
        {
            var instance = Object.Instantiate(prefab);

            Object.DontDestroyOnLoad(instance);

            instance.name = "Popup";

            var script = Dependancy.Get<PopupUI>(instance);

            return script;
        }
    }
}