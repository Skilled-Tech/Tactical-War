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

        public Core Core => Core.Instance;
        public PlayerCore Player => Core.Player;
        public LocalizationCore Localization => Core.Localization;

        public Funds.ElementData Element => Player.Funds.Find(type);

        void Awake()
        {
            label = GetComponent<TMP_Text>();
        }

        void OnEnable()
        {
            Player.Funds.OnValueChanged += OnChange;
            Localization.OnTargetChange += LocalizationTargetChanged;

            UpdateState();
        }

        void UpdateState()
        {
            label.text = Element.Value.ToString("N0");
        }

        void LocalizationTargetChanged(LocalizationType target)
        {
            UpdateState();
        }

        void OnChange()
        {
            UpdateState();
        }

        void OnDisable()
        {
            Player.Funds.OnValueChanged -= OnChange;
            Localization.OnTargetChange -= LocalizationTargetChanged;
        }
    }
}