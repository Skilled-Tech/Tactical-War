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
    public class SlotsIndicator : MonoBehaviour
    {
        [SerializeField]
        protected Image[] images;

        [SerializeField]
        Color on = Color.green;

        [SerializeField]
        Color off = Color.red;

        [SerializeField]
        protected bool deActivate;
        public bool DeActivate { get { return deActivate; } }

        public int Value
        {
            set
            {
                for (int i = 0; i < images.Length; i++)
                {
                    if(deActivate) images[i].gameObject.SetActive(i >= value ? false : true);

                    images[i].color = i >= value ? off : on;
                }
            }
        }
    }
}