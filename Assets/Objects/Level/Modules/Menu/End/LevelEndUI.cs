﻿using System;
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
using TMPro;

namespace Game
{
	public class LevelEndUI : LevelMenu.Element
	{
		[SerializeField]
        protected TMP_Text info;
        public TMP_Text Info { get { return info; } }

        [SerializeField]
        protected Button next;
        public Button Next { get { return next; } }

        [SerializeField]
        protected Button retry;
        public Button Retry { get { return retry; } }

        public Core Core { get { return Core.Instance; } }

        public virtual void Show(Proponent winner)
        {
            if (winner is PlayerProponent)
            {
                info.text = "You Won";

                next.gameObject.SetActive(!Level.Data.Level.IsLast);
                retry.gameObject.SetActive(false);
            }
            else
            {
                info.text = "You Lost";

                next.gameObject.SetActive(false);
                retry.gameObject.SetActive(true);
            }

            Show();
        }
    }
}