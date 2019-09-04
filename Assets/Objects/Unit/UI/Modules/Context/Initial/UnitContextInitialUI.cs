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

            description.text = Data.Template.Description;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            Unlock.gameObject.SetActive(!Data.Instance.Unlocked);

            Upgrade.gameObject.SetActive(Data.Instance.Unlocked);

            if (Data.Instance.Unlocked)
            {
                
            }
            else
            {
                unlock.interactable = Player.Funds.CanAfford(Data.Template.Unlock.Cost);

                price.color = unlock.interactable ? Color.white : Color.grey;

                price.text = Currency.FormatText(0, (int)Data.Template.CatalogItem.VirtualCurrencyPrices[Core.Player.Funds.Jewels.Code]);
            }
        }

        void UnlockClick()
        {
            Context.Character.Slot.gameObject.SetActive(false);

            Popup.Show("Processing Purchase");

            Core.PlayFab.Purchase.OnResponse += OnPurchaseResponse;
            Core.PlayFab.Purchase.Perform(Data.Template.CatalogItem, Core.Player.Funds.Jewels.Code);
        }

        void OnPurchaseResponse(PlayFab.ClientModels.PurchaseItemResult result, PlayFab.PlayFabError error)
        {
            Core.PlayFab.Purchase.OnResponse -= OnPurchaseResponse;

            if(error == null)
            {
                Popup.Show("Retrieving Inventory");

                Core.PlayFab.Inventory.OnResponse += OnInventoryResponse;
                Core.PlayFab.Inventory.Request();
            }
            else
            {
                Popup.Show(error.ErrorMessage, Popup.Hide, "Close");
            }
        }

        void OnInventoryResponse(PlayFabInventoryCore result, PlayFab.PlayFabError error)
        {
            Core.PlayFab.Inventory.OnResponse -= OnInventoryResponse;

            if (error == null)
            {
                Popup.Hide();

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