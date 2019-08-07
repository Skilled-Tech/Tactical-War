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
        protected ProgressBar cooldownBar;
        public ProgressBar CooldownBar { get { return cooldownBar; } } 

        public Button button;

        void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);

            UpdateState();

            proponent.Ability.OnSelectionChanged += OnAbilitySelectionChanged;
            proponent.Funds.OnValueChanged += OnFundsChanged;
            proponent.Ability.Cooldown.OnStateChange += OnStateChange;
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
            button.interactable = proponent.Ability.CanUse;

            if (proponent.Ability.Cooldown.Rate == 0f)
                cooldownBar.Value = 1f;
            else
                cooldownBar.Value = proponent.Ability.Cooldown.Rate;
        }
    }
}