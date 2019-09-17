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

        void Awake()
        {
            Label = GetComponent<TMP_Text>();
        }

        void Start()
        {
            Proponent.Energy.OnChanged += UpdateState;

            UpdateState();
        }

        void UpdateState()
        {
            Label.text = "Energy: " + Proponent.Energy.Value.ToString("N0");
        }
    }
}