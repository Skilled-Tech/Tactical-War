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

namespace Game
{
    [RequireComponent(typeof(Button))]
	public class ButtonRelay : Relay
	{
        Button button;

        protected override void Awake()
        {
            base.Awake();

            button = GetComponent<Button>();

            button.onClick.AddListener(Invoke);
        }
    }
}