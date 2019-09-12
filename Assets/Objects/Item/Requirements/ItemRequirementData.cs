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
using Newtonsoft.Json;

namespace Game
{
    [Serializable]
    public class ItemRequirementData
    {
        [JsonProperty]
        [JsonConverter(typeof(ItemTemplate.Converter))]
        [SerializeField]
        protected ItemTemplate item;
        public ItemTemplate Item { get { return item; } }

        [JsonProperty]
        [SerializeField]
        protected int count;
        public int Count { get { return count; } }

        public static Core Core { get { return Core.Instance; } }
        public static ItemsCore Items { get { return Core.Items; } }

        public ItemRequirementData()
        {

        }
    }
}