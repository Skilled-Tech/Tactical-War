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
    [RequireComponent(typeof(Image))]
	public class LocalizedImage : LocalizationBehaviour.DataModifer<Image, Sprite, LocalizedImage.Element>
    {
        public override Sprite Value
        {
            get
            {
                return Component.sprite;
            }
            protected set
            {
                Component.sprite = value;
            }
        }

        protected override void Reset()
        {
            base.Reset();

            IList array = Enum.GetValues(typeof(LocalizationType));

            elements = new Element[array.Count];

            for (int i = 0; i < array.Count; i++)
            {
                elements[i] = new Element((LocalizationType)array[i], Value);
            }
        }

        [Serializable]
        public class Element : Element<Sprite>
        {
            public Element(LocalizationType localization, Sprite value)
            {
                this.localization = localization;
                this.value = value;
            }
        }
    }
}