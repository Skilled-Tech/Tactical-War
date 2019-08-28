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

        public Core Core { get { return Core.Instance; } }
        public PlayerCore Player { get { return Core.Player; } }

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
            Player.Funds.OnValueChanged += UpdateState;

            UpdateState();
        }

        void UpdateState()
        {
            button.interactable = Player.Funds.CanAfford(Target.Turret.Cost);

            price.text = Target.Cost.ToString();
        }

        void OnButton()
        {
            Target.Turret.isDeployed = true;

            Context.Hide();
        }

        void OnDisable()
        {
            Player.Funds.OnValueChanged -= UpdateState;
        }
    }
}