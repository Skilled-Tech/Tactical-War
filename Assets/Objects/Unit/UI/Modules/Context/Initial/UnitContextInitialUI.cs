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
using PlayFab;

namespace Game
{
    public class UnitContextInitialUI : UnitContextUI.Module
    {
        [SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

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

        public PlayFabCore PlayFab { get { return Core.PlayFab; } }

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

        public override void UpdateState()
        {
            base.UpdateState();

            Template.Icon.ApplyTo(icon);

            description.text = Template.Description;

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

        #region Unlock
        void UnlockClick()
        {
            Popup.Show("Processing Purchase");

            Core.PlayFab.Purchase.OnResponse += PurchaseResponse;
            Core.PlayFab.Purchase.Perform(Template.CatalogItem);
        }

        void PurchaseResponse(PurchaseItemResult result, PlayFabError error)
        {
            Core.PlayFab.Purchase.OnResponse -= PurchaseResponse;

            if (error == null)
            {
                Popup.Show("Retrieving Inventory");

                PlayFab.Player.Inventory.OnResponse += OnInventoryResponse;
                PlayFab.Player.Inventory.Request();
            }
            else
            {
                RaiseError(error);
            }
        }

        void OnInventoryResponse(PlayFabPlayerInventoryCore result, PlayFab.PlayFabError error)
        {
            PlayFab.Player.Inventory.OnResponse -= OnInventoryResponse;

            if (error == null)
            {
                Popup.Hide();

                UpdateState();
            }
            else
            {
                RaiseError(error);
            }
        }
        #endregion

        void RaiseError(PlayFabError error)
        {
            Popup.Show(error.ErrorMessage, Popup.Hide, "Close");
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