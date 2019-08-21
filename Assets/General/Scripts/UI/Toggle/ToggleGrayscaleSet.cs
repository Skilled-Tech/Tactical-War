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
    public class ToggleGrayscaleSet : ToggleSet<float>
    {
        Graphic graphic;

        void Reset()
        {
            on = 0f;
            off = 1f;
        }

        protected override void Awake()
        {
            base.Awake();

            graphic = GetComponent<Graphic>();

            graphic.material = Instantiate(graphic.material);
        }

        protected override void Set(float value)
        {
            graphic.material.SetFloat("_EffectAmount", value);
        }
    }
}