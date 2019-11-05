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
	public class LoadingMenu : MonoBehaviour
	{
        [SerializeField]
        protected Image background;
        public Image Background { get { return background; } }

        [SerializeField]
        protected ProgressBar progress;
        public ProgressBar Progress { get { return progress; } }

        [SerializeField]
        protected Sprite[] sprites;
        public Sprite[] Sprites { get { return sprites; } }

        public Core Core => Core.Instance;

        private void Start()
        {
            Core.PlayFab.EnsureActivation();

            background.sprite = sprites[Random.Range(0, sprites.Length)];
        }

        private void Update()
        {
            progress.Value = Core.Scenes.Load.Progress;
        }
    }
}