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
	public class EntityStatusEffectUITemplate : UIElement
	{
		[SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        public virtual void Init()
        {

        }

        public StatusEffectInstance Effect { get; protected set; }
        public virtual void Set(StatusEffectInstance effect)
        {
            this.Effect = effect;

            icon.color = effect.Type.Color;
            icon.sprite = effect.Type.Icon;
        }
    }
}