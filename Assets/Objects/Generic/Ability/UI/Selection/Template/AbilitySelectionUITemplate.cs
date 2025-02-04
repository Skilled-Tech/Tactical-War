﻿using System;
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

using UnityEngine.EventSystems;

namespace Game
{
	public class AbilitySelectionUITemplate : AbilityUITemplate, IPointerEnterHandler, IPointerExitHandler
	{
        [SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        public RateTransition Transition { get; protected set; }

        new public AbilityTemplate Target
        {
            get
            {
                return Selection[Index];
            }
            set
            {
                Selection[Index] = value;

                Set(Target);
            }
        }

        public PlayerAbilitySelectionCore Selection { get { return Core.Player.Ability.Selection; } }

        public int Index { get; protected set; }

        public virtual void Init(int index)
        {
            this.Index = index;

            Transition = new RateTransition(this, 1f);
            Transition.Speed = 15f;
            Transition.OnValueChanged += OnTransition;

            base.Init();

            Selection.OnChange += SelectionChangeCallback;
        }

        void OnTransition(float value)
        {
            CanvasGroup.alpha = Mathf.Lerp(0.65f, 1f, value);

            RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x, Mathf.Lerp(-RectTransform.sizeDelta.y / 3f, 0f, value));
        }

        void OnEnable()
        {
            StartCoroutine(Procedure());

            IEnumerator Procedure()
            {
                yield return new WaitForEndOfFrame();

                Transition.Value = 1f;

                yield return new WaitForSeconds(0.35f);

                Transition.To(0f);
            }
        }

        void SelectionChangeCallback(int index, AbilityTemplate target)
        {
            if (this.Index == index)
            {
                Set(Target);
            }
        }

        public override void Set(AbilityTemplate data)
        {
            if(data == null)
            {
                base.Template = data;

                label.text = (Index + 1).ToString();
            }
            else
            {
                base.Set(data);
            }

            icon.gameObject.SetActive(data != null);
            label.gameObject.SetActive(data == null);

            GrayscaleController.On = data == null;
        }

        protected override void Click()
        {
            base.Click();

            Target = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Transition.To(1f);

            RectTransform.SetAsLastSibling();

            if (Selection.Context.Template == null)
            {

            }
            else
            {
                Set(Selection.Context.Template);

                Selection.Context.SetSlot(Index);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Transition.To(0f);

            RectTransform.SetSiblingIndex(Index);

            if (Selection.Context.Template == null)
            {

            }
            else
            {
                Set(Target);

                Selection.Context.SetSlot(null);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Selection.OnChange -= SelectionChangeCallback;
        }
    }
}