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
	public class EntityDefense : Entity.Module
	{
		[SerializeField]
        [Range(0f, 100f)]
        protected float _value = 0f;
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                value = Mathf.Clamp(value, 0f, 100f);

                _value = value;
            }
        }

        public virtual float Sample(float damage)
        {
            return Mathf.Lerp(damage, 0f, Value / 100f);
        }
    }
}