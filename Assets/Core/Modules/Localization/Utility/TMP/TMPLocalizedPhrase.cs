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
    [RequireComponent(typeof(TMP_Text))]
    public class TMPLocalizedPhrase : LocalizationBehaviour.PhraseLabel<TMP_Text>
    {
        public override string Text
        {
            get
            {
                return Component.text;
            }
            protected set
            {
                Component.text = value;
            }
        }
    }
}