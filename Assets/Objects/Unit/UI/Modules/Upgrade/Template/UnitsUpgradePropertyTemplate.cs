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
    [RequireComponent(typeof(UIGrayscaleController))]
	public class UnitsUpgradePropertyTemplate : MonoBehaviour
	{
        [SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        [SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        [SerializeField]
        protected SlotsIndicator progression;
        public SlotsIndicator Progression { get { return progression; } }

        [SerializeField]
        protected Button button;
        public Button Button { get { return button; } }

        [SerializeField]
        protected TMP_Text price;
        public TMP_Text Price { get { return price; } }

        public UIGrayscaleController GrayscaleController { get; protected set; }

        public Core Core { get { return Core.Instance; } }
        public Funds Funds { get { return Core.Player.Funds; } }

        public UnitUpgradesController.TypeController Reference { get; protected set; }
        public virtual void Set(UnitUpgradesController.TypeController reference)
        {
            this.Reference = reference;

            reference.OnUpgrade += OnUpgrade;

            GrayscaleController = GetComponent<UIGrayscaleController>();
            GrayscaleController.Init();

            icon.sprite = reference.Target.Icon;

            label.text = reference.Target.name + " Upgrade";

            button.onClick.AddListener(OnButon);

            Funds.OnValueChanged += OnFundsChanged;

            UpdateState();
        }

        void OnUpgrade()
        {
            UpdateState();
        }

        void OnFundsChanged()
        {
            UpdateState();
        }

        void UpdateState()
        {
            if (Reference.Maxed)
            {
                button.interactable = false;

                price.text = "FULL";
            }
            else
            {
                price.text = Reference.Next.Cost.ToString();

                button.interactable = Reference.CanUpgrade(Funds) && !Reference.Maxed;
            }

            progression.Value = (int)Reference.Index;

            GrayscaleController.On = !button.interactable;

            price.color = button.interactable ? Color.white : Color.gray;
        }

        void OnButon()
        {
            Reference.Upgrade(Funds);
        }

        void OnDestroy()
        {
            Reference.OnUpgrade -= OnUpgrade;

            Funds.OnValueChanged -= OnFundsChanged;
        }
    }
}