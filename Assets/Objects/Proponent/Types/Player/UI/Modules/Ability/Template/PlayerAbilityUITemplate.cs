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

namespace Game
{
    [RequireComponent(typeof(Button))]
    public class PlayerAbilityUITemplate : MonoBehaviour
    {
        [SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        [SerializeField]
        protected Image background;
        public Image Background { get { return background; } }

        public UIGrayscaleController GrayscaleController { get; protected set; }

        public Button Button { get; protected set; }

        public ProgressBar CooldownBar { get; protected set; }

        public Core Core { get { return Core.Instance; } }

        public Level Level { get { return Level.Instance; } }
        public Proponent Proponent { get { return Level.Proponents.Player; } }

        public virtual void Init()
        {
            Button = GetComponent<Button>();

            CooldownBar = Dependancy.Get<ProgressBar>(gameObject);

            GrayscaleController = new UIGrayscaleController(this);

            Button.onClick.AddListener(OnClick);

            CooldownBar.Value = 0f;

            Proponent.Energy.OnValueChanged += OnEnergyChanged;
        }

        public ProponentAbility Ability { get; protected set; }
        public virtual void Set(ProponentAbility reference)
        {
            this.Ability = reference;

            Ability.Cooldown.OnTick += ElementCoolDownTickCallback;

            UpdateState();
        }

        void OnClick()
        {
            Ability.Use();
        }

        private void ElementCoolDownTickCallback()
        {
            UpdateState();
        }
        void OnEnergyChanged()
        {
            UpdateState();
        }

        void UpdateState()
        {
            Ability.Template.Icon.ApplyTo(icon);

            Button.interactable = Ability.CanUse;

            if (Ability.Cooldown.Timer == 0f)
            {
                CooldownBar.Value = 0f;
                GrayscaleController.Ammount = 0f;
            }
            else
            {
                CooldownBar.Value = Ability.Cooldown.Rate;
                GrayscaleController.Ammount = 1f;
            }
        }

        protected virtual void OnDestroy()
        {
            Proponent.Energy.OnValueChanged -= OnEnergyChanged;
            Ability.Cooldown.OnTick -= ElementCoolDownTickCallback;
        }
    }
}