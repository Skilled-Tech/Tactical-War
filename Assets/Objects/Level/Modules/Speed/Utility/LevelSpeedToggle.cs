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
    [RequireComponent(typeof(Toggle))]
	public class LevelSpeedToggle : MonoBehaviour
	{
        Toggle toggle;

        [SerializeField]
        protected float target = 4f;
        public float Target { get { return target; } } 

        public LevelSpeed Speed { get { return Level.Instance.Speed; } }

        void Start()
        {
            toggle = GetComponent<Toggle>();

            toggle.onValueChanged.AddListener(OnToggle);
        }

        void OnToggle(bool isOn)
        {
            if (isOn)
                Speed.Value = target;
            else
                Speed.Value = 1f;
        }
    }
}