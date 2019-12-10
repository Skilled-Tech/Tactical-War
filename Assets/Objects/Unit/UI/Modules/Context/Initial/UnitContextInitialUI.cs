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
        protected UnitUITemplate icon;
        public UnitUITemplate Icon { get { return icon; } }

        [SerializeField]
        protected TMP_Text description;
        public TMP_Text Description { get { return description; } }

        [SerializeField]
        protected StatsData stats;
        public StatsData Stats { get { return stats; } }
        [Serializable]
        public class StatsData
        {
            [SerializeField]
            protected TMP_Text power;
            public TMP_Text Power { get { return power; } }

            [SerializeField]
            protected TMP_Text range;
            public TMP_Text Range { get { return range; } }

            [SerializeField]
            protected TMP_Text defense;
            public TMP_Text Defense { get { return defense; } }

            [SerializeField]
            protected TMP_Text hp;
            public TMP_Text HP { get { return hp; } }

            [SerializeField]
            protected TMP_Text speed;
            public TMP_Text Speed { get { return speed; } }

            [SerializeField]
            protected TMP_Text deploy;
            public TMP_Text Deploy { get { return deploy; } }

            public virtual void Init()
            {

            }

            public virtual void Set(UnitTemplate template)
            {
                FormatUpgradeLabel(power, template, Core.Items.Upgrades.Types.Common.Power, template.Attack.Power);
                FormatUpgradeLabel(range, template, Core.Items.Upgrades.Types.Common.Range, template.Attack.Range);
                FormatUpgradeLabel(defense, template, Core.Items.Upgrades.Types.Common.Defense, template.Defense);

                FormatLabel(hp, nameof(HP), template.Health);
                FormatLabel(speed, nameof(Speed), template.MovementMethod);
                FormatLabel(deploy, nameof(Deploy), template.Deployment.Time, "seconds-unit");
            }

            public virtual void FormatUpgradeLabel(TMP_Text label, ItemTemplate item, ItemUpgradeType type, float value)
            {
                Player.Units.Upgrades.GetElements(item, type, out var template, out var data);

                if(item.Upgrades.isApplicable(type))
                {
                    label.gameObject.SetActive(true);

                    if (template == null || data == null)
                    {
                        FormatLabel(label, type.name, value);
                    }
                    else
                    {
                        if (data.Value == 0)
                        {
                            FormatLabel(label, type.name, value);
                        }
                        else
                        {
                            FormatLabel(label, type.name, value * template.Ranks[data.Index].Multiplier);
                        }
                    }
                }
                else
                {
                    label.gameObject.SetActive(false);
                }
            }

            protected virtual void FormatLabel(TMP_Text label, string text, object value) => FormatLabel(label, text, value, "");
            protected virtual void FormatLabel(TMP_Text label, string text, object value, string unit)
            {
                text = Core.Localization.Phrases.Get(text);

                if(string.IsNullOrEmpty(unit) == false)
                    unit = Core.Localization.Phrases.Get(unit);

                label.text = text + ": " + value.ToString() + unit;
            }
        }

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

            icon.Init();
            icon.Lock.Active = false;

            stats.Init();

            unlock.onClick.AddListener(UnlockClick);
            upgrade.onClick.AddListener(UpgradeClick);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Player.Funds.OnValueChanged += UpdateState;
            Player.Inventory.OnUpdate += InventoryUpdateCallback;
        }

        private void InventoryUpdateCallback()
        {
            UpdateState();
        }

        public override void UpdateState()
        {
            base.UpdateState();

            icon.Set(Template);

            description.text = Template.Description.Text;

            var unlocked = Player.Inventory.Contains(Template.CatalogItem);

            Unlock.gameObject.SetActive(!unlocked);
            Upgrade.gameObject.SetActive(unlocked);

            stats.Set(Template);

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
            Core.UI.Buy.Show(Template);
        }

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
            Player.Inventory.OnUpdate -= InventoryUpdateCallback;
        }
    }
}