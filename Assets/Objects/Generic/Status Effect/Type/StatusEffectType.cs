﻿using System;
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
	public abstract class StatusEffectType : ScriptableObject
	{
        public const string MenuPath = StatusEffect.MenuPath + "Type/";

        [SerializeField]
        protected Color color = Color.white;
        public Color Color { get { return color; } }

        [SerializeField]
        protected Sprite icon;
        public Sprite Icon { get { return icon; } }

        [SerializeField]
        protected StatusEffectType[] conflicts;
        public StatusEffectType[] Conflicts { get { return conflicts; } }
        public virtual bool ConflictsWith(StatusEffectType type)
        {
            return conflicts.Contains(type);
        }

        public virtual void OnAdd(StatusEffectInstance effect)
        {

        }

        public abstract void Apply(StatusEffectInstance effect);

        public virtual void OnRemove(StatusEffectInstance effect)
        {

        }
	}
}