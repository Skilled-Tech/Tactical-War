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
	public class BaseTowerSlotsUseContextUI : BaseTowerSlotContextUI.Element
    {
        [SerializeField]
        protected ProponentBuyButton button;
        public ProponentBuyButton Button { get { return button; } } 

        protected override bool IsApplicaple(BaseTowerSlot slot)
        {
            return slot.isDeployed && !slot.Turret.isDeployed;
        }

        public override void Init()
        {
            base.Init();

            button.Init();
            button.OnPurchase += OnPurchase;
        }

        public override void Show()
        {
            base.Show();

            button.Cost = Target.Turret.Cost;
        }

        void OnPurchase()
        {
            Target.Turret.isDeployed = true;

            Context.Hide();
        }
    }
}