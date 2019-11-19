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
    [RequireComponent(typeof(Image))]
	public class RandomImageSprite : MonoBehaviour
	{
        public Image Image { get; protected set; }

        [SerializeField]
        protected Sprite[] sprites;
        public Sprite[] Sprites { get { return sprites; } }

        private void Start()
        {
            Image = GetComponent<Image>();

            Image.sprite = sprites[Random.Range(0, sprites.Length)];
        }
    }
}