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
    public class ProponentEnergyLabel : MonoBehaviour
    {
        public TMP_Text Label { get; protected set; }

        public Proponent Proponent { get { return Level.Instance.Proponents.Player; } }

        public Core Core => Core.Instance;

        void Awake()
        {
            Label = GetComponent<TMP_Text>();
        }

        void Start()
        {
            Proponent.Energy.OnChanged += UpdateState;

            Core.Localization.OnTargetChange += LocalizationTargetChangeCallback;

            UpdateState();
        }

        private void LocalizationTargetChangeCallback(LocalizationType target)
        {
            UpdateState();
        }

        void UpdateState()
        {
            Label.text = Proponent.Energy.Value.ToString("N0") + " " + Core.Localization.Phrases.Get("energy");
        }

        private void OnDestroy()
        {
            Core.Localization.OnTargetChange -= LocalizationTargetChangeCallback;
        }
    }
}