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

        new public RectTransform transform { get; protected set; }

        public Core Core { get { return Core.Instance; } }

        public PlayerUnitsSelectionCore SelectionCore { get { return Core.Player.Units.Selection; } }

        new public UnitData Target
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
        public UnitData Context { get { return SelectionCore.Context; } }

        public int Index { get; protected set; }
        public virtual void Init(int index)
        {
            this.Index = index;

            Init();
        }
        public override void Init()
        {
            transform = GetComponent<RectTransform>();

            base.Init();
        }
        
        void OnEnable()
        {
            StartCoroutine(OnEnableProcedure());
        }
        IEnumerator OnEnableProcedure()
        {
            yield return new WaitForEndOfFrame();

            Transition = 1f;

            yield return new WaitForSeconds(0.35f);

            TransitionTo(0f);
        }

        public override void Set(UnitData data)
        {
            if(data == null)
            {
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
            TransitionTo(1f);

            if (Context == null)
            {

            }
            else
            {
                Target = Context;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TransitionTo(0f);

            if (Context == null)
            {

            }
            else
            {
                Target = null;
            }
        }

        public float _transition = 0f;
        public virtual float Transition
        {
            get
            {
                return _transition;
            }
            set
            {
                value = Mathf.Clamp01(value);

                _transition = value;

                CanvasGroup.alpha = Mathf.Lerp(0.6f, 1f, Transition);

                var position = transform.anchoredPosition;
                position.y = Mathf.Lerp(-transform.sizeDelta.y * 0.8f, -transform.sizeDelta.y / 2f, Transition);
                transform.anchoredPosition = position;
            }
        }

        void TransitionTo(float height)
        {
            if (transitionToCoroutine != null)
                StopCoroutine(transitionToCoroutine);

            transitionToCoroutine = StartCoroutine(TransitionToProcedure(height));
        }

        Coroutine transitionToCoroutine;
        IEnumerator TransitionToProcedure(float target)
        {
            var position = transform.localPosition;

            while(Transition != target)
            {
                Transition = Mathf.MoveTowards(Transition, target, 15 * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }

            transitionToCoroutine = null;
        }
    }
}