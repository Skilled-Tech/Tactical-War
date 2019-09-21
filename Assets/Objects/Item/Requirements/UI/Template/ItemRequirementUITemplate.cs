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

using PlayFab.ClientModels;

namespace Game
{
	public class ItemRequirementUITemplate : MonoBehaviour
	{
        [SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        [SerializeField]
        protected TMP_Text uses;
        public TMP_Text Uses { get { return uses; } }

        public UIGrayscaleController Grayscale { get; protected set; }

		public virtual void Init()
        {
            Grayscale = new UIGrayscaleController(this);
        }

        public virtual void Set(ItemTemplate template, uint count)
        {
            template.Icon.ApplyTo(icon);

            uses.text = "x" + count.ToString();
        }
	}
}