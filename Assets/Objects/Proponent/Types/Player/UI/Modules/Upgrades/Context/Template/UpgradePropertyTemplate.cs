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
	public class UpgradePropertyTemplate : UIElement
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

        public ProponentUpgradeProperty Reference { get; protected set; }
        public virtual void Set(ProponentUpgradeProperty reference)
        {
            this.Reference = reference;

            icon.sprite = reference.Type.Icon;

            label.text = reference.Type.name + " Upgrade";

            button.onClick.AddListener(OnButon);

            Reference.Proponent.Funds.OnValueChanged += OnFundsChanged;

            UpdateState();
        }

        void OnFundsChanged()
        {
            UpdateState();
        }

        void OnButon()
        {
            Reference.Upgrade();

            UpdateState();
        }

        void UpdateState()
        {
            if(Reference.Maxed)
            {
                price.text = "FULL";
            }
            else
            {
                price.text = Reference.Next.Cost.ToString();
            }

            progression.Value = (int)Reference.Index;

            button.interactable = Reference.CanAfford && !Reference.Maxed;
        }

        void OnDestroy()
        {
            Reference.Proponent.Funds.OnValueChanged -= OnFundsChanged;
        }
    }
}