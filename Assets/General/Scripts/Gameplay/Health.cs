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
	public class Health : MonoBehaviour
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
                value = Mathf.Clamp(value, 0f, Max);

                _value = value;

                if (OnValueChanged != null) OnValueChanged(Value);
            }
        }

        public event Action<float> OnValueChanged;

        [SerializeField]
        protected float _max = 100;
        public float Max
        {
            get
            {
                return _max;
            }
            set
            {
                if (value < 0f) value = 0f;

                _max = value;
            }
        }

        public float Rate { get { return Value / Max; } }

        protected virtual void Reset()
        {
            _value = _max;
        }
    }
}