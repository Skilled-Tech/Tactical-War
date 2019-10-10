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
	public class ShopSectionUI : UIElement
	{
        [SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        [SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        public virtual void Init()
        {

        }

        public ShopSectionCore Section { get; protected set; }

        public virtual void Set(ShopSectionCore section)
        {
            this.Section = section;

            label.text = section.name;

            if(icon != null)
                icon.sprite = section.Icon;
        }
    }
}