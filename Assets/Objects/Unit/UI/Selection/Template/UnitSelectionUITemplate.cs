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

using UnityEngine.EventSystems;

namespace Game
{
	public class UnitSelectionUITemplate : UnitUITemplate, IPointerEnterHandler, IPointerExitHandler
	{
        [SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        public RateTransition Transition { get; protected set; }

        public PlayerUnitsSelectionCore SelectionCore { get { return Core.Player.Units.Selection; } }

        new public UnitTemplate Target
        {
            get
            {
                return SelectionCore[Index];
            }
            set
            {
                SelectionCore[Index] = value;

                Set(Target);
            }
        }
        public UnitTemplate Context { get { return SelectionCore.Context; } }

        public int Index { get; protected set; }

        public virtual void Init(int index)
        {
            this.Index = index;

            Transition = new RateTransition(this, 1f);
            Transition.Speed = 15f;
            Transition.OnValueChanged += OnTransition;

            Init();
        }

        void OnTransition(float value)
        {
            CanvasGroup.alpha = Mathf.Lerp(0.4f, 1f, value);

            RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x, Mathf.Lerp(-RectTransform.sizeDelta.y / 3f, 0f, value));
        }

        public void OnTemplateDragEnd()
        {
            if (Template != Target) Target = Template;
        }

        void OnEnable()
        {
            StartCoroutine(OnEnableProcedure());
        }
        IEnumerator OnEnableProcedure()
        {
            yield return new WaitForEndOfFrame();

            Transition.Value = 1f;

            yield return new WaitForSeconds(0.35f);

            Transition.To(0f);
        }

        public override void Set(UnitTemplate data)
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

            if (Context == null)
            {

            }
            else
            {
                Set(Context);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Transition.To(0f);

            RectTransform.SetSiblingIndex(Index);

            if (Context == null)
            {

            }
            else
            {
                Set(Target);
            }
        }
    }
}