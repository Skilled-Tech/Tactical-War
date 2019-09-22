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
    public class ItemStack
    {
        [JsonProperty]
        [JsonConverter(typeof(ItemTemplate.Converter))]
        [SerializeField]
        protected ItemTemplate item;
        public ItemTemplate Item { get { return item; } }

        [JsonProperty]
        [SerializeField]
        protected uint count;
        public uint Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
            }
        }

        public static Core Core { get { return Core.Instance; } }
        public static ItemsCore Items { get { return Core.Items; } }

        public override string ToString()
        {
            return "x" + count + " " + item.name;
        }

        public ItemStack()
        {

        }

        public ItemStack(ItemTemplate item, uint count)
        {
            this.item = item;
            this.count = count;
        }
        public ItemStack(ItemTemplate template) : this(template, 1)
        {

        }

        //Static Utility
        public static List<ItemStack> From(IList<string> IDs)
        {
            string AquireID(string ID) => ID;

            return From(IDs, AquireID);
        }
        public static List<ItemStack> From(IList<ItemTemplate> templates)
        {
            string AquireID(ItemTemplate template) => template.ID;

            return From(templates, AquireID);
        }
        public static List<ItemStack> From<T>(IList<T> list, Func<T, string> aquireID)
        {
            var result = new List<ItemStack>();

            ItemStack Find(string ID)
            {
                for (int i = 0; i < result.Count; i++)
                    if (result[i].Item.ID == ID)
                        return result[i];

                return null;
            }

            for (int i = 0; i < list.Count; i++)
            {
                var ID = aquireID(list[i]);
                var existing = Find(ID);

                if (existing == null)
                {
                    var template = Items.Find(ID);

                    if (template == null)
                    {

                    }
                    else
                    {
                        result.Add(new ItemStack(template));
                    }
                }
                else
                {
                    existing.Count++;
                }
            }

            return result;
        }
    }
}