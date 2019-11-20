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

            label.text = type.name + " Upgrade";

            UpdateState();
        }

        void UpdateState()
        {
            var data = Player.Units.Upgrades.Find(Template).Find(Type);
            var template = Template.Upgrades.Template.Find(Type);

            var isFull = data.Value >= template.Ranks.Length;

            instruction.gameObject.SetActive(isFull);
            buy.gameObject.SetActive(!isFull);

            if (isFull)
            {
                buy.interactable = false;

                requiremnets.Hide();

                instruction.text = data.Value + "/" + template.Ranks.Length + " Full";
            }
            else
            {
                var nextRank = template.Ranks[data.Value];

                price.text = nextRank.Cost.ToString();

                buy.interactable = Funds.CanAfford(nextRank.Cost) && Player.Inventory.CompliesWith(nextRank.Requirements);

                requiremnets.Set(nextRank.Requirements);
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