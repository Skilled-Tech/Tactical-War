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
    [RequireComponent(typeof(ProgressBar))]
	public class EntityHealthBar : MonoBehaviour
	{
        [SerializeField]
        protected Entity target;
        public Entity Target { get { return target; } } 

        ProgressBar bar;

        protected virtual void Start()
        {
            bar = GetComponent<ProgressBar>();

            target.Health.OnValueChanged += OnChange;

            bar.Value = target.Health.Rate;
        }

        void OnChange(float obj)
        {
            bar.Value = target.Health.Rate;
        }
    }
}