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
    [ExecuteInEditMode]
    [RequireComponent(typeof(LayoutElement))]
    [RequireComponent(typeof(RectTransform))]
    public class LoadingIndicator : MonoBehaviour
	{
        [SerializeField]
        private LayoutElement layoutElement;

        [SerializeField]
        private RectTransform rectTransform;

        [SerializeField]
        private float aspectRation = 0f;

        private void Reset()
        {
            layoutElement = GetComponent<LayoutElement>();

            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            layoutElement.minHeight = rectTransform.sizeDelta.x * aspectRation;
        }
    }
}