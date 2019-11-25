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

using TMPro;
using RTLTMPro;

namespace Game
{
    [Serializable]
	public class LocalizationPresetsCore : LocalizationCore.Property
	{
        [SerializeField]
        protected LocalizationPreset[] elements;
        public LocalizationPreset[] Elements { get { return elements; } }

        public int Count => elements.Length;
        public LocalizationPreset this[int index] => elements[index];

        public LocalizationPreset Current => Get(Localization.Target);

        public virtual LocalizationPreset Get(LocalizationType type)
        {
            for (int i = 0; i < Count; i++)
                if (this[i].Type == type)
                    return this[i];

            return null;
        }
    }

    [Serializable]
    public class LocalizationPreset
    {
        [SerializeField]
        protected LocalizationType type;
        public LocalizationType Type { get { return type; } }

        [SerializeField]
        protected TMP_FontAsset font;
        public TMP_FontAsset Font { get { return font; } }
    }
}