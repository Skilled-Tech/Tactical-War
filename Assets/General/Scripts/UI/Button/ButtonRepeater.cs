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

using UnityEngine.EventSystems;

namespace Game
{
    [RequireComponent(typeof(Button))]
	public class ButtonRepeater : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
        [SerializeField]
        protected DelayData delay = new DelayData(1f, 0.4f);
        public DelayData Delay { get { return delay; } }
        [Serializable]
        public class DelayData
        {
            [SerializeField]
            protected float initial;
            public float Initial { get { return initial; } }


            [SerializeField]
            protected float progressive;
            public float Progressive { get { return progressive; } }

            public DelayData(float initial, float progressive)
            {
                this.initial = initial;
                this.progressive = progressive;
            }
        }

        Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(pointerID.HasValue)
            {

            }
            else
            {
                pointerID = eventData.pointerId;

                coroutine = StartCoroutine(Procedure());
            }
        }

        int? pointerID;
        Coroutine coroutine;
        IEnumerator Procedure()
        {
            yield return new WaitForSeconds(delay.Initial);

            while(true)
            {
                if(button.interactable) button.onClick.Invoke();

                yield return new WaitForSeconds(delay.Progressive);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (pointerID.HasValue)
            {
                if(pointerID.Value == eventData.pointerId)
                {
                    if (coroutine != null)
                        StopCoroutine(coroutine);

                    pointerID = null;
                }
            }
            else
            {

            }
        }
    }
}