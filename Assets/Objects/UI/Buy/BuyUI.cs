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
	public class BuyUI : UIElement
	{
		[SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

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
        protected Button button;
        public Button Button { get { return button; } }

        public Core Core { get { return Core.Instance; } }

        public PlayFabCore PlayFab { get { return Core.PlayFab; } }

        public PopupUI Popup { get { return Core.UI.Popup; } }

        public virtual void Init()
        {
            button.onClick.AddListener(ButtonAction);
        }

        public ItemTemplate Template { get; protected set; }

        public virtual void Show(ItemTemplate template)
        {
            this.Template = template;

            UpdateState();

            Show();
        }

        public virtual void UpdateState()
        {
            Template.Icon.ApplyTo(icon);

            label.text = Template.DisplayName.Text;

            description.text = Template.Description.Text;

            price.text = Template.Price.ToString();
        }

        protected virtual void ButtonAction()
        {
            if(PlayFab.isOffline)
            {
                Core.UI.ShowOnlineRequirementPopup();
            }
            else
            {
                Popup.Show(Core.Localization.Phrases.Get("Processing Purchase"));

                if (Template.Price.Type == CurrencyType.Cents)
                {
                    Core.IAP.Purchase(Template.ID);
                }
                else
                {
                    PlayFab.Purchase.OnResponse += PurchaseResponse;
                    PlayFab.Purchase.Perform(Template.CatalogItem);
                }
            }
        }

        void PurchaseResponse(PurchaseItemResult result, PlayFabError error)
        {
            Core.PlayFab.Purchase.OnResponse -= PurchaseResponse;

            if (error == null)
            {
                Popup.Show(Core.Localization.Phrases.Get("Retrieving Inventory"));

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
                void Action()
                {
                    Popup.Hide();

                    Hide();
                }

                Popup.Show(Core.Localization.Phrases.Get("Purchase Successful"), Action, Core.Localization.Phrases.Get("Okay"));

                UpdateState();
            }
            else
            {
                RaiseError(error);
            }
        }

        void RaiseError(PlayFabError error)
        {
            Popup.Show(error.ErrorMessage, Popup.Hide, "Close");
        }
    }
}