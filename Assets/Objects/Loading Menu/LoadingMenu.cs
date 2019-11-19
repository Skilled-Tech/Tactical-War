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
using TMPro;

namespace Game
{
	public class LoadingMenu : MonoBehaviour, IPointerClickHandler
	{
        [SerializeField]
        protected ProgressData progress;
        public ProgressData Progress { get { return progress; } }
        [Serializable]
        public class ProgressData
        {
            [SerializeField]
            protected TMP_Text label;
            public TMP_Text Label { get { return label; } }

            [SerializeField]
            protected ProgressBar bar;
            public ProgressBar Bar { get { return bar; } }
        }

        public Core Core => Core.Instance;

        private void Start()
        {
            Core.PlayFab.EnsureActivation();
        }

        private void Update()
        {
            progress.Bar.Value = Core.Scenes.Load.Progress;
            progress.Label.text = (Core.Scenes.Load.Progress * 100).ToString("N0") + "%";
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
        }
    }
}