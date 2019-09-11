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

using Newtonsoft.Json.Linq;

namespace Game
{
    [Serializable]
    public class ItemRequirementData
    {
        [SerializeField]
        protected ItemTemplate item;
        public ItemTemplate Item { get { return item; } }

        [SerializeField]
        protected int count;
        public int Count { get { return count; } }

        public static Core Core { get { return Core.Instance; } }
        public static ItemsCore Items { get { return Core.Items; } }

        public virtual void Load(JToken token)
        {
            item = Items.Find(token[nameof(Item)].ToObject<string>());

            count = token[nameof(Count)].ToObject<int>();
        }

        public ItemRequirementData(JToken token)
        {
            Load(token);
        }
        public ItemRequirementData(ItemTemplate item, int count)
        {
            this.item = item;
            this.count = count;
        }
    }
}