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
    [RequireComponent(typeof(Button))]
	public class ProponentBuyButton : MonoBehaviour
	{
        [SerializeField]
        protected Proponent target;
        public Proponent Target { get { return target; } } 

        [SerializeField]
        protected Currency cost;
        public Currency Cost
        {
            get
            {
                return cost;
            }
            set
            {
                cost = value;

                UpdateState();
            }
        }

        [SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } } 

        public string Text { get { return label.text; } set { label.text = value; } }

        Button button;

        public virtual void Init()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);

            target.Funds.OnValueChanged += OnFundsChanged;
        }

        public virtual void UpdateState()
        {
            Text = cost.ToString();

            button.interactable = target.Funds.CanAfford(cost);
        }

        public event Action OnPurchase;
        void OnClick()
        {
            target.Funds.Take(cost);

            if (OnPurchase != null) OnPurchase();
        }

        void OnFundsChanged()
        {
            UpdateState();
        }
    }
}