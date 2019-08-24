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
	public class ProponentAbilityUseButton : MonoBehaviour
	{
        [SerializeField]
        protected Proponent proponent;
        public Proponent Proponent { get { return proponent; } } 

        [SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } } 

        [SerializeField]
        protected Image background;
        public Image Background { get { return background; } }

        public float Grayscale
        {
            set
            {
                icon.material.SetFloat("_EffectAmount", value);

                background.material.SetFloat("_EffectAmount", value);
            }
        }

        public Button Button { get; protected set; }

        public ProgressBar CooldownBar { get; protected set; }

        void Start()
        {
            Button = GetComponent<Button>();
            Button.onClick.AddListener(OnClick);

            CooldownBar = Dependancy.Get<ProgressBar>(gameObject);
            CooldownBar.Value = 0f;

            UpdateState();

            proponent.Ability.OnSelectionChanged += OnAbilitySelectionChanged;
            proponent.Funds.OnValueChanged += OnFundsChanged;
            proponent.Ability.Cooldown.OnStateChange += OnStateChange;

            icon.material = Instantiate(icon.material);
            background.material = Instantiate(background.material);
        }

        void OnClick()
        {
            proponent.Ability.Use();
        }

        void OnAbilitySelectionChanged(Ability ability)
        {
            UpdateState();
        }
        void OnStateChange()
        {
            UpdateState();
        }
        void OnFundsChanged()
        {
            UpdateState();
        }

        void UpdateState()
        {
            Button.interactable = proponent.Ability.CanUse;

            if (proponent.Ability.Cooldown.Rate == 0f)
            {
                CooldownBar.Value = 0f;
                Grayscale = 0f;
            }
            else
            {
                CooldownBar.Value = proponent.Ability.Cooldown.Rate;
                Grayscale = 1f;
            }
        }

        void OnDestroy()
        {
            proponent.Ability.OnSelectionChanged -= OnAbilitySelectionChanged;
            proponent.Funds.OnValueChanged -= OnFundsChanged;
            proponent.Ability.Cooldown.OnStateChange -= OnStateChange;
        }
    }
}