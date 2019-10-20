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
        protected StatsData stats;
        public StatsData Stats { get { return stats; } }
        [Serializable]
        public class StatsData
        {
            [SerializeField]
            protected AttackProperty attack;
            public AttackProperty Attack { get { return attack; } }
            [Serializable]
            public class AttackProperty : Property
            {
                [SerializeField]
                protected TMP_Text label;
                public TMP_Text Label { get { return label; } }

                protected override float GetBaseValue(UnitTemplate template) => template.Attack.Power;

                public override void Apply(UnitTemplate template, float value)
                {
                    label.text = "Attack: " + value.ToString();

                    label.gameObject.SetActive(template.Upgrades.isApplicable(upgrade));
                }
            }

            [SerializeField]
            protected RangeProperty range;
            public RangeProperty Range { get { return range; } }
            [Serializable]
            public class RangeProperty : Property
            {
                [SerializeField]
                protected TMP_Text label;
                public TMP_Text Label { get { return label; } }

                protected override float GetBaseValue(UnitTemplate template) => template.Attack.Range;

                public override void Apply(UnitTemplate template, float value)
                {
                    label.text = "Range: " + value.ToString();

                    label.gameObject.SetActive(template.Upgrades.isApplicable(upgrade));
                }
            }

            [SerializeField]
            protected DefenseProperty defense;
            public DefenseProperty Defense { get { return defense; } }
            [Serializable]
            public class DefenseProperty : Property
            {
                [SerializeField]
                protected TMP_Text label;
                public TMP_Text Label { get { return label; } }

                protected override float GetBaseValue(UnitTemplate template) => 0f;

                public override void Apply(UnitTemplate template, float value)
                {
                    label.text = "Defense: " + value.ToString();

                    label.gameObject.SetActive(template.Upgrades.isApplicable(upgrade));
                }
            }

            [Serializable]
            public abstract class Property
            {
                [SerializeField]
                protected ItemUpgradeType upgrade;
                public ItemUpgradeType Upgrade { get { return upgrade; } }

                public virtual void UpdateState(UnitTemplate unit)
                {
                    var rank = GetRank(unit);

                    var percentage = rank == null ? 0f : rank.Percentage;

                    var multiplier = 1 + (percentage / 100f);

                    var value = GetBaseValue(unit) * multiplier;

                    Apply(unit, value);
                }

                public abstract void Apply(UnitTemplate template, float value);

                public Core Core { get { return Core.Instance; } }
                public PlayerCore Player { get { return Core.Player; } }

                protected abstract float GetBaseValue(UnitTemplate template);
                protected virtual ItemUpgradesData.ElementData GetData(UnitTemplate unit)
                {
                    var data = Player.Units.Upgrades.Find(unit);

                    if (data == null) return null;

                    return data.Find(upgrade);
                }
                protected ItemUpgradesTemplate.ElementData GetTemplate(UnitTemplate unit)
                {
                    var result = unit.Upgrades.Template.Find(upgrade);

                    return result;
                }
                public ItemUpgradesTemplate.ElementData.RankData GetRank(UnitTemplate unit)
                {
                    var data = GetData(unit);
                    if (data == null) return null;

                    var template = GetTemplate(unit);
                    if (template == null) return null;

                    if (data.Value == 0) return null;

                    return template.Ranks[data.Value - 1];
                }
            }

            [SerializeField]
            protected TMP_Text hp;
            public TMP_Text HP { get { return hp; } }

            [SerializeField]
            protected TMP_Text speed;
            public TMP_Text Speed { get { return speed; } }

            [SerializeField]
            protected TMP_Text cooldown;
            public TMP_Text Cooldown { get { return cooldown; } }

            public virtual void Init()
            {

            }

            public virtual void Set(UnitTemplate template)
            {
                attack.UpdateState(template);
                range.UpdateState(template);
                defense.UpdateState(template);

                UpdateText(hp, nameof(HP), template.Health);
                UpdateText(speed, nameof(Speed), template.Speed);
                UpdateText(cooldown, nameof(Cooldown), template.Deployment.Time);
            }

            protected virtual void UpdateText(TMP_Text text, string label, object value)
            {
                text.text = label + ": " + value.ToString();
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

            stats.Init();

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