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
	public class UICoreMenu : UIElement
	{
		[SerializeField]
        protected RewardsUI rewards;
        public RewardsUI Rewards { get { return rewards; } }

        [SerializeField]
        protected PopupUI popup;
        public PopupUI Popup { get { return popup; } }

        [SerializeField]
        protected BuyUI buy;
        public BuyUI Buy { get { return buy; } }

        public Core Core => Core.Instance;

        public virtual void Init()
        {
            buy.Init();
            rewards.Init();

            rewards.Hide();
            popup.Hide();
            buy.Hide();
        }

        private void Update()
        {
            if (Application.isEditor && Input.GetKeyDown(KeyCode.Space))
            {
                Core.Localization.Progress();
            }
        }
    }
}