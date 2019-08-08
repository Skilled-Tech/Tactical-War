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
	public class LevelSpeed : Level.Module
	{
        [SerializeField]
        protected float _value;
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value < 0f) value = 0f;

                _value = value;

                Time.timeScale = Value;
            }
        }

        public override void Init()
        {
            base.Init();


        }


    }
}