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
	public class EntityTimeScale : Entity.Module
	{
        //0% to 100%
		public virtual float Rate
        {
            get
            {
                var percentage = 0f;

                for (int i = 0; i < Modifiers.Count; i++)
                    percentage += Modifiers[i].Percentage;

                return 1f + Mathf.LerpUnclamped(0f, 1f, percentage / 100f);
            }
        }

        public List<IModifer> Modifiers { get; protected set; }

        public interface IModifer
        {
            /// <summary>
            /// To be implemeted as a value from 0% to 100% or beyond if you're into things not working :)
            /// </summary>
            float Percentage { get; }
        }

        public override void Init()
        {
            base.Init();

            Modifiers = Dependancy.GetAll<IModifer>(Entity.gameObject);
        }
    }
}