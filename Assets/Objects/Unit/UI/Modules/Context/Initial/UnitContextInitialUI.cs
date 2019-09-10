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
	public class UnitContextInitialUI : UnitContextUI.Module
    {
        [SerializeField]
        protected TMP_Text description;
        public TMP_Text Description { get { return description; } }

        [SerializeField]
        protected TMP_Text price;
        public TMP_Text Price { get { return price; } }

        [SerializeField]
        protected Button unlock;
        public Button Unlock { get { return unlock; } }

        [SerializeField]
        protected Button upgrade;
        public Button Upgrade { get { return upgrade; } }

        public PopupUI Popup { get { return MainMenu.Instance.Popup; } }

        public override void Init()
        {
            base.Init();

            unlock.onClick.AddListener(UnlockClick);
            upgrade.onClick.AddListener(UpgradeClick);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Player.Funds.OnValueChanged += UpdateState;
        }

        public override void Show()
        {
            base.Show();

            description.text = Template.Description;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            var unlocked = Player.Inventory.Contains(Template.CatalogItem);

            Unlock.gameObject.SetActive(!unlocked);

            Upgrade.gameObject.SetActive(unlocked);

            if (unlocked)
            {
                
            }
            else
            {
                unlock.interactable = Player.Funds.CanAfford(Template.Price);

                price.color = unlock.interactable ? Color.white : Color.grey;

                price.text = Template.Price.ToString();
            }
        }

        void UnlockClick()
        {
            Context.Character.Slot.gameObject.SetActive(false);

            Popup.Show("Processing Purchase");

            Core.PlayFab.Purchase.OnResponse += OnPurchaseResponse;
            Core.PlayFab.Purchase.Perform(Template.CatalogItem, Core.Player.Funds.Jewels.Code);
        }

        void OnPurchaseResponse(PlayFabPurchaseCore purchase, PurchaseItemResult result, PlayFab.PlayFabError error)
        {
            Core.PlayFab.Purchase.OnResponse -= OnPurchaseResponse;

            if (error == null)
            {
                Popup.Show("Retrieving Inventory");

                Player.Inventory.OnResponse += OnInventoryResponse;
                Player.Inventory.Request();
            }
            else
            {
                Popup.Show(error.ErrorMessage, Popup.Hide, "Close");
            }
        }

        void OnInventoryResponse(PlayerInventoryCore result, PlayFab.PlayFabError error)
        {
            Player.Inventory.OnResponse -= OnInventoryResponse;

            if (error == null)
            {
                Popup.Hide();

                UpdateState();

                Context.Character.Slot.gameObject.SetActive(true);
            }
            else
            {
                Popup.Show(error.ErrorMessage, Popup.Hide, "Close");
            }
        }

        void UpgradeClick()
        {
            Hide();

            Context.Upgrade.Show();
        }

        void OnDisable()
        {
            Player.Funds.OnValueChanged -= UpdateState;
        }
    }
}