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
	public class RateTransition
	{
        public MonoBehaviour Behaviour { get; protected set; }

        protected float _value;
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                value = Mathf.Clamp01(value);

                _value = value;

                if (OnValueChanged != null) OnValueChanged(Value);
            }
        }

        protected float _speed = 1f;
        public float Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                if (value < 0f) value = 0f;

                _speed = value;
            }
        }

        public event Action<float> OnValueChanged;

        public virtual void To(float target)
        {
            if (coroutine != null)
                Behaviour.StopCoroutine(coroutine);

            coroutine = Behaviour.StartCoroutine(Procedure(target));
        }
        public Coroutine coroutine;
        public IEnumerator Procedure(float target)
        {
            while(Value != target)
            {
                Value = Mathf.MoveTowards(Value, target, Speed * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }

            coroutine = null;
        }

        public RateTransition(MonoBehaviour behaviour, float value)
        {
            this.Behaviour = behaviour;

            this.Value = value;
        }
        public RateTransition(MonoBehaviour behaviour) : this(behaviour, 0f)
        {

        }
	}
}