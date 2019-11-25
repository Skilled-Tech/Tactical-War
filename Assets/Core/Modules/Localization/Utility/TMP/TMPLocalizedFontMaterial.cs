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
    public class TMPLocalizedFontMaterial : LocalizationBehaviour.DataModifer<TMP_Text, Material, TMPLocalizedFontMaterial.Element>
    {
        public override Material Value
        {
            get
            {
                return Component.fontSharedMaterial;
            }
            protected set
            {
                Component.fontSharedMaterial = value;
            }
        }

        protected override void Reset()
        {
            base.Reset();

            elements = new Element[1];
            elements[0] = new Element(LocalizationType.English, Value);
        }

        [Serializable]
        public class Element : Element<Material>
        {
            public Element(LocalizationType type, Material value) : base(type, value) { }
        }
    }
}