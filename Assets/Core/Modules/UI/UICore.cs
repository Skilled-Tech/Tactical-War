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
	public class UICore : Core.Property
	{
        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        public UICoreMenu Menu { get; protected set; }

        public OptionsMenu OptionsMenu => Menu.OptionsMenu;
        public PopupUI Popup { get { return Menu.Popup; } }
        public RewardsUI Rewards { get { return Menu.Rewards; } }
        public BuyUI Buy => Menu.Buy;
        public DialogUI Dialog => Menu.Dialog;
        public FaderUI Fader => Menu.Fader;

        [Serializable]
        public class Module : Core.Property
        {
            public UICore UI { get { return Core.UI; } }

            public UICoreMenu Menu { get; protected set; }
        }

        public override void Configure()
        {
            base.Configure();

            Menu = Create();
        }

        UICoreMenu Create()
        {
            var instance = Object.Instantiate(prefab);

            Object.DontDestroyOnLoad(instance);

            instance.name = "UI Core Menu";

            var script = Dependancy.Get<UICoreMenu>(instance);

            script.Init();

            return script;
        }

        public void ShowOnlineRequirementPopup()
        {
            Popup.Show(Core.Localization.Phrases.Get("Need Online Login"), Popup.Hide, Core.Localization.Phrases.Get("Okay"));
        }
    }
}