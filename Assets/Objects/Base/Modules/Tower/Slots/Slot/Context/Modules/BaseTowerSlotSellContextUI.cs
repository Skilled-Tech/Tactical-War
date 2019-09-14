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
    public class BaseTowerSlotSellContextUI : BaseTowerSlotContextUI.Element
    {
        [SerializeField]
        protected Button button;
        public Button Button { get { return button; } }

        [SerializeField]
        protected TMP_Text gain;
        public TMP_Text Gain { get { return gain; } }

        protected override bool IsApplicaple(BaseTowerSlot slot)
        {
            return slot.isDeployed && slot.Turret.isDeployed;
        }

        public override void Init()
        {
            base.Init();

            button.onClick.AddListener(OnButton);
        }

        public override void Show()
        {
            base.Show();

            gain.text = Target.Turret.Cost.ToString();
        }

        void OnButton()
        {
            Target.Turret.isDeployed = false;

            Target.Proponent.Energy.Value += Target.Turret.Cost;

            UpdateTarget();
        }
    }
}