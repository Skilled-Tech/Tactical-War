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
using PlayFab.ClientModels;

namespace Game
{
    public class ItemUpgradePropertyUITemplate : UIElement
    {
        [SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        [SerializeField]
        protected SlotsIndicator progression;
        public SlotsIndicator Progression { get { return progression; } }

        [SerializeField]
        protected ItemStacksUI requiremnets;
        public ItemStacksUI Requirements { get { return requiremnets; } }

        [SerializeField]
        protected Button buy;
        public Button Buy { get { return buy; } }
        public TMP_Text BuyLabel { get; protected set; }

        [SerializeField]
        protected TMP_Text price;
        public TMP_Text Price { get { return price; } }

        public UIGrayscaleController GrayscaleController { get; protected set; }

        public Core Core { get { return Core.Instance; } }

        public PlayerCore Player { get { return Core.Player; } }
        public Funds Funds { get { return Player.Funds; } }

        public PlayFabCore PlayFab { get { return Core.PlayFab; } }

        public PopupUI Popup { get { return Core.UI.Popup; } }

        void OnEnable()
        {
            Funds.OnValueChanged += UpdateState;
        }

        public virtual void Init()
        {
            GrayscaleController = new UIGrayscaleController(this);

            requiremnets.Init();

            buy.onClick.AddListener(OnButon);

            BuyLabel = buy.GetComponentInChildren<TMP_Text>();
        }

        public UnitTemplate Template { get; protected set; }
        public ItemUpgradeType Type { get; protected set; }
        public virtual void Set(UnitTemplate template, ItemUpgradeType type)
        {
            this.Template = template;
            this.Type = type;

            label.text = type.name + " Upgrade";

            UpdateState();
        }

        void UpdateState()
        {
            var data = Player.Units.Upgrades.Find(Template).Find(Type);
            var template = Template.Upgrades.Template.Find(Type);

            if (data.Value >= template.Ranks.Length)
            {
                buy.interactable = false;

                price.gameObject.SetActive(false);

                requiremnets.Set(null);
            }
            else
            {
                var rank = template.Ranks[data.Value];

                price.gameObject.SetActive(true);
                price.text = "Cost: " + rank.Cost.ToString();

                buy.interactable = Funds.CanAfford(rank.Cost) && Player.Inventory.CompliesWith(rank.Requirements);

                requiremnets.Set(rank.Requirements);
            }

            progression.Value = data.Value;

            GrayscaleController.On = !buy.interactable;

            BuyLabel.color = buy.interactable ? Color.white : Color.Lerp(Color.white, Color.black, 0.75f);
        }

        void OnButon()
        {
            var data = Player.Inventory.Find(Template.CatalogItem);

            Popup.Show("Processing Upgrade");

            PlayFab.Upgrade.OnResponse += OnResponse;
            PlayFab.Upgrade.Perform(data, Type);
        }

        void OnResponse(ExecuteCloudScriptResult result, PlayFab.PlayFabError error)
        {
            PlayFab.Upgrade.OnResponse -= OnResponse;

            if (error == null)
            {
                Popup.Show("Retriving Inventory");

                PlayFab.Player.Inventory.Request();
                PlayFab.Player.Inventory.OnResponse += OnInventoryResponse;
            }
            else
            {
                Popup.Show(error.ErrorMessage, Popup.Hide, "Close");
            }
        }

        void OnInventoryResponse(PlayFabPlayerInventoryCore inventory, PlayFab.PlayFabError error)
        {
            PlayFab.Player.Inventory.OnResponse -= OnInventoryResponse;

            if (error == null)
            {
                Popup.Hide();

                UpdateState();
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