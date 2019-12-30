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
    public class BaseTowerSlotsUseContextUI : BaseTowerSlotContextUI.Element
    {
        [SerializeField]
        protected Button button;
        public Button Button { get { return button; } }

        [SerializeField]
        protected TMP_Text price;
        public TMP_Text Price { get { return price; } }

        protected override bool IsApplicaple(BaseTowerSlot slot)
        {
            return slot.isDeployed && !slot.Turret.isDeployed;
        }

        public override void Init()
        {
            base.Init();

            button.onClick.AddListener(OnButton);
        }

        void OnEnable()
        {
            Proponent.Energy.OnValueChanged += UpdateState;

            UpdateState();
        }

        void UpdateState()
        {
            button.interactable = Proponent.Energy.Value >= Target.Turret.Cost;

            price.text = Target.Cost.ToString();
        }

        void OnButton()
        {
            Target.Turret.isDeployed = true;

            Context.Hide();
        }

        void OnDisable()
        {
            Proponent.Energy.OnValueChanged -= UpdateState;
        }
    }
}