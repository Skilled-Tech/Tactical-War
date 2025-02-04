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

namespace Game
{
    [CreateAssetMenu(menuName = LevelBackground.MenuPath + "Data")]
	public class LevelBackgroundData : ScriptableObject
	{
        [SerializeField]
        protected int copies = 3;
        public int Copies { get { return copies; } }

        [SerializeField]
        protected ElementData[] elements;
        public ElementData[] Elements { get { return elements; } }
        [Serializable]
        public class ElementData
        {
            [SerializeField]
            protected GameObject prefab;
            public GameObject Prefab { get { return prefab; } }

            [SerializeField]
            protected BackgroundAnchor anchor = BackgroundAnchor.Center;
            public BackgroundAnchor Anchor { get { return anchor; } }

            [SerializeField]
            protected float height;
            public float Height { get { return height; } }

            [SerializeField]
            protected float parallax;
            public float Parallax { get { return parallax; } }
        }
	}

    public enum BackgroundAnchor
    {
        Center,
        Bottom,
        Top,
    }
}