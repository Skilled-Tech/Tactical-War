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
    [Serializable]
	public class ShopSectionCore : ShopCore.Module
	{
        [SerializeField]
        protected string name;
        public string Name { get { return name; } }

        [SerializeField]
        protected List<ItemTemplate> items;
        public List<ItemTemplate> Items { get { return items; } }
    }
}