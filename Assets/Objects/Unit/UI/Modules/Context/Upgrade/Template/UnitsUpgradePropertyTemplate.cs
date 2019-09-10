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

        void OnEnable()
        {
            Funds.OnValueChanged += UpdateState;
        }

        public virtual void Init()
        {
            GrayscaleController = new UIGrayscaleController(this);
        }

        public UnitTemplate Template { get; protected set; }
        public ItemUpgradeType Type { get; protected set; }
        public virtual void Set(UnitTemplate template, ItemUpgradeType type)
        {
            this.Template = template;
            
            //TODO
            /*
            icon.sprite = Template.Data.Icon;
            label.text = Template.Data.name + " Upgrade";
            */

            button.onClick.AddListener(OnButon);

            UpdateState();
        }

        void UpdateState()
        {
            //TODO
            /*
            if (Data.Maxed)
            {
                button.interactable = false;

                price.text = "FULL";
            }
            else
            {
                price.text = Data.Next.Cost.ToString();

                button.interactable = Data.CanUpgrade(Funds) && !Data.Maxed;
            }

            progression.Value = (int)Data.Value;

            GrayscaleController.On = !button.interactable;

            price.color = button.interactable ? Color.white : Color.Lerp(Color.white, Color.black, 0.75f);
            */
        }

        void OnButon()
        {
            //TODO
        }

        void OnDisable()
        {
            Funds.OnValueChanged -= UpdateState;
        }
    }
}