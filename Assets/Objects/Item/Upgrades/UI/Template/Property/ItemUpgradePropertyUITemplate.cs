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
using PlayFab.ClientModels;

namespace Game
{
    public class ItemUpgradePropertyUITemplate : UIElement
    {
        [SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        [SerializeField]
        protected TMP_Text instruction;
        public TMP_Text Instruction { get { return instruction; } }

        [SerializeField]
        protected SlotsIndicator progression;
        public SlotsIndicator Progression { get { return progression; } }

        [SerializeField]
        protected ItemStacksUI requiremnets;
        public ItemStacksUI Requirements { get { return requiremnets; } }

        [SerializeField]
        protected Button buy;
        public Button Buy { get { return buy; } }

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
        }

        public UnitTemplate Template { get; protected set; }
        public ItemUpgradeType Type { get; protected set; }
        public virtual void Set(UnitTemplate template, ItemUpgradeType type)
        {
            this.Template = template;
            this.Type = type;

            label.text = type.DisplayName.Text;

            UpdateState();
        }

        void UpdateState()
        {
            var data = Player.Units.Upgrades.Find(Template).Find(Type);
            var template = Template.Upgrades.Template.Find(Type);

            var isFull = data.Value >= ItemsUpgradesCore.Max;

            instruction.gameObject.SetActive(isFull);
            //buy.gameObject.SetActive(!isFull);

            if (isFull)
            {
                buy.interactable = false;

                price.text = "---------";

                requiremnets.Hide();

                instruction.text = data.Value + "/" + ItemsUpgradesCore.Max + " " + Core.Localization.Phrases.Get("Full");
            }
            else
            {
                var cost = new Currency(ItemsUpgradesCore.Currency, (int)template.Cost.Calculate(data.Value + 1));
                var requirements = template.Requirements[data.Value];

                price.text = cost.ToString();

                buy.interactable = Funds.CanAfford(cost) && Player.Inventory.CompliesWith(requirements);

                requiremnets.Set(requirements);
            }

            progression.Value = data.Value;

            GrayscaleController.Off = buy.interactable;
        }

        void OnButon()
        {
            if(PlayFab.isOffline)
            {
                Core.UI.ShowOnlineRequirementPopup();
            }
            else
            {
                var data = Player.Inventory.Find(Template.CatalogItem);

                Popup.Show(Core.Localization.Phrases.Get("Processing Upgrade"));

                PlayFab.Upgrade.OnResponse += OnResponse;
                PlayFab.Upgrade.Perform(data, Type);
            }
        }

        void OnResponse(PlayFabUpgradeCore.ResultData result, PlayFab.PlayFabError error)
        {
            PlayFab.Upgrade.OnResponse -= OnResponse;

            if (error == null)
            {
                Popup.Show(Core.Localization.Phrases.Get("Retrieving Inventory"));

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