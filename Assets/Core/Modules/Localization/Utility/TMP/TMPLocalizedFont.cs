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

namespace Game
{
    public class TMPLocalizedFont : LocalizationBehaviour.DataModifer<TMP_Text, TMP_FontAsset, TMPLocalizedFont.Element>
    {
        public override TMP_FontAsset Value
        {
            get
            {
                return Component.font;
            }
            protected set
            {
                Component.font = value;
            }
        }

        protected override void Start()
        {
            base.Start();

            Debug.Log(name);
        }

        [Serializable]
        public class Element : Element<TMP_FontAsset> { }
    }
}