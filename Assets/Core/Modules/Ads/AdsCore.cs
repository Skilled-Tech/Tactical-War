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

using UnityEngine.Advertisements;

namespace Game
{
    [Serializable]
	public class AdsCore : Core.Property
	{
        [SerializeField]
        protected string gameID;
        public string GameID { get { return gameID; } }

        public bool TestMode
        {
            get
            {
                if (Application.isEditor) return true;

                return false;
            }
        }

        [SerializeField]
        protected ItemTemplate disableItem;
        public ItemTemplate DisableItem { get { return disableItem; } }

        public virtual bool Ignore
        {
            get
            {
                return Core.Player.Inventory.Contains(disableItem);
            }
        }
        public virtual bool Active => !Ignore;

        public override void Configure()
        {
            base.Configure();

            Advertisement.Initialize(gameID, TestMode);
        }
    }
}