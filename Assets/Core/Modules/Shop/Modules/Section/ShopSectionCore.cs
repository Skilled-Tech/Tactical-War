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
    [CreateAssetMenu(menuName = MenuPath + "Section")]
	public class ShopSectionCore : ShopCore.Module
	{
        public string ID { get { return name; } }

        public static bool CompareID(string id1, string id2)
        {
            return String.Equals(id1, id2, StringComparison.OrdinalIgnoreCase);
        }

        public LocalizedPhraseProperty DisplayName { get; protected set; }

        [SerializeField]
        protected IconProperty icon;
        public IconProperty Icon { get { return icon; } }

        [SerializeField]
        protected List<ItemTemplate> items;
        public List<ItemTemplate> Items { get { return items; } }

        public override void Init()
        {
            base.Init();

            DisplayName = LocalizedPhraseProperty.Create(ID);
        }
    }
}