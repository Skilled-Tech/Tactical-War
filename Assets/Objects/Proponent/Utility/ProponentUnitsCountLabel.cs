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
    [RequireComponent(typeof(TMP_Text))]
	public class ProponentUnitsCountLabel : MonoBehaviour
	{
        [SerializeField]
        protected Proponent target;
        public Proponent Target { get { return target; } } 

        TMP_Text label;

        void Start()
        {
            label = GetComponent<TMP_Text>();

            Target.Base.Units.OnCountChanged += OnChange;

            UpdateState();
        }

        void UpdateState()
        {
            label.text = target.Base.Units.Count + "/" + target.Base.Units.Max + " Units";
        }

        void OnChange(int value)
        {
            UpdateState();
        }
    }
}