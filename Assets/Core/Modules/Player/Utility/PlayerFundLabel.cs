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
    public class PlayerFundLabel : MonoBehaviour
    {
        [SerializeField]
        protected CurrencyType type;
        public CurrencyType Type { get { return type; } }

        TMP_Text label;

        public PlayerCore Player { get { return Core.Instance.Player; } }

        void Awake()
        {
            label = GetComponent<TMP_Text>();
        }

        void OnEnable()
        {
            Player.Funds.OnValueChanged += OnChange;

            UpdateState();
        }

        void UpdateState()
        {
            label.text = Player.Funds.Find(type).Value.ToString("N0") + " " + type.ToString();
        }

        void OnChange()
        {
            UpdateState();
        }

        void OnDisable()
        {
            Player.Funds.OnValueChanged -= OnChange;
        }
    }
}