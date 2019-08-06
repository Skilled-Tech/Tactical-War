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
	public class ProponentFundLabel : MonoBehaviour
	{
        [SerializeField]
        protected Proponent target;
        public Proponent Target { get { return target; } }

        [SerializeField]
        protected CurrencyType type;
        public CurrencyType Type { get { return type; } }

        TMP_Text label;

        void Start()
        {
            label = GetComponent<TMP_Text>();

            target.Funds.OnValueChanged += OnChange;

            UpdateState();
        }

        void UpdateState()
        {
            label.text = target.Funds.Get(type).Value.ToString("N0") + " " + type.ToString();
        }

        void OnChange()
        {
            UpdateState();
        }
    }
}