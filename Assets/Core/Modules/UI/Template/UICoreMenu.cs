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

        public virtual void Init()
        {
            rewards.Init();
        }
    }
}