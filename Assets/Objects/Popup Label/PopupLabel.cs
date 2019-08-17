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

namespace Game
{
    [RequireComponent(typeof(TMP_Text))]
	public class PopupLabel : MonoBehaviour
	{
        TMP_Text label;

        public string Text { get { return label.text; } set { label.text = value; } }

        public float Transparency
        {
            get
            {
                return label.alpha;
            }
            set
            {
                label.alpha = value;
            }
        }

        public Color Color { get { return label.color; } set { label.color = value; } }

        [SerializeField]
        protected float duration = 2f;
        public float Duration { get { return duration; } } 
        
        [SerializeField]
        protected float distance = 2f;
        public float Distance { get { return distance; } }

        public void Configure()
        {
            label = GetComponent<TMP_Text>();

            StartCoroutine(Procedure());
        }

        IEnumerator Procedure()
        {
            var timer = duration;

            var initialPosition = transform.position;
            var endPosition = initialPosition + Vector3.up * distance;

            while(timer > 0f)
            {
                timer = Mathf.MoveTowards(timer, 0f, Time.deltaTime);

                Transparency = timer / duration * 4f;

                transform.position = Vector3.Lerp(endPosition, initialPosition, timer / duration);

                yield return new WaitForEndOfFrame();
            }

            Destroy(gameObject);
        }
    }
}