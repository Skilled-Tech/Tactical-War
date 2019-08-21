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
    public class ToggleColorSet : ToggleSet<Color>
    {
        Graphic graphic;

        void Reset()
        {
            on = Color.white;
            off = Color.white;
        }

        protected override void Awake()
        {
            base.Awake();

            graphic = GetComponent<Graphic>();
        }

        protected override void Set(Color value)
        {
            graphic.color = value;
        }
    }
}