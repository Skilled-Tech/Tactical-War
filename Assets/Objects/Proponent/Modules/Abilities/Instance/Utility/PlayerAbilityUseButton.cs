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
    public class PlayerAbilityUseButton : MonoBehaviour
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

        [SerializeField]
        protected ProponentAbility target;
        public ProponentAbility Target { get { return target; } }

        public Level Level { get { return Level.Instance; } }
        public Proponent Proponent { get { return Level.Proponents.Player; } }

        void Awake()
        {
            Button = GetComponent<Button>();

            CooldownBar = Dependancy.Get<ProgressBar>(gameObject);

            GrayscaleController = new UIGrayscaleController(this);
        }

        void Start()
        {
            Button.onClick.AddListener(OnClick);

            CooldownBar.Value = 0f;

            icon.material = Instantiate(icon.material);
            background.material = Instantiate(background.material);

            Proponent.Energy.OnChanged += OnEnergyChanged;
            target.Cooldown.OnStateChange += OnCoolDownStateChange;
        }

        void OnEnable()
        {
            UpdateState();
        }

        void OnClick()
        {
            target.Use();
        }

        void OnCoolDownStateChange()
        {
            UpdateState();
        }
        void OnEnergyChanged()
        {
            UpdateState();
        }

        void UpdateState()
        {
            Button.interactable = target.CanUse;

            if (target.Cooldown.Rate == 0f)
            {
                CooldownBar.Value = 0f;
                GrayscaleController.Ammount = 0f;
            }
            else
            {
                CooldownBar.Value = target.Cooldown.Rate;
                GrayscaleController.Ammount = 1f;
            }
        }

        void OnDestroy()
        {
            Proponent.Energy.OnChanged -= OnEnergyChanged;
            target.Cooldown.OnStateChange -= OnCoolDownStateChange;
        }
    }
}