﻿using System;
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

        public PlayerCore Player { get { return Core.Player; } }
        public Funds Funds { get { return Player.Funds; } }

        public PlayFabCore PlayFab { get { return Core.PlayFab; } }

        public PopupUI Popup { get { return MainMenu.Instance.Popup; } }

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
            
            icon.sprite = type.Icon;
            label.text = type.name + " Upgrade";

            button.onClick.AddListener(OnButon);

            UpdateState();
        }

        void UpdateState()
        {
            var data = Player.Units.Upgrades.Find(Template).Find(Type);
            var template = Template.Upgrades.Template.Find(Type);

            if (data.Value >= template.Ranks.Length)
            {
                button.interactable = false;

                price.text = "FULL";
            }
            else
            {
                price.text = template.Ranks[data.Value].Cost.ToString();

                button.interactable = Funds.CanAfford(template.Ranks[data.Value].Cost);
            }

            progression.Value = data.Value;

            GrayscaleController.On = !button.interactable;

            price.color = button.interactable ? Color.white : Color.Lerp(Color.white, Color.black, 0.75f);
        }

        void OnButon()
        {
            var instance = Player.Inventory.Find(Template.CatalogItem);

            Popup.Show("Processing Upgrade");

            PlayFab.Upgrade.OnResponse += OnResponse;
            PlayFab.Upgrade.Perform(instance, Funds.Jewels.Code);
        }

        void OnResponse(PlayFabUpgradeCore upgrade, PlayFab.ClientModels.ExecuteCloudScriptResult result, PlayFab.PlayFabError error)
        {
            PlayFab.Upgrade.OnResponse -= OnResponse;

            if (error == null)
            {
                Popup.Show("Retriving Inventory");

                Player.Inventory.Request();
                Player.Inventory.OnResponse += OnInventoryResponse;
            }
            else
            {
                Popup.Show(error.ErrorMessage, Popup.Hide, "Close");
            }
        }

        void OnInventoryResponse(PlayerInventoryCore inventory, PlayFab.PlayFabError error)
        {
            Player.Inventory.OnResponse -= OnInventoryResponse;

            if (error == null)
            {
                Popup.Hide();
            }
            else
            {
                Popup.Show(error.ErrorMessage, Popup.Hide, "Close");
            }
        }

        void OnDisable()
        {
            Funds.OnValueChanged -= UpdateState;
        }
    }
}