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
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        protected UIElement title;
        public UIElement Title { get { return title; } }

        [SerializeField]
        protected UIElement start;
        public UIElement Start { get { return start; } }

        [SerializeField]
        protected UIElement credits;
        public UIElement Credits { get { return credits; } }

        [SerializeField]
        protected UIElement units;
        public UIElement Units { get { return units; } }
    }
}