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

using UnityEngine.Scripting;

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

        [Preserve]
        public class TextConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return typeof(ItemTemplate).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.Value == null) return null;

                var text = reader.Value as string;

                return ItemStack.From(text);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value == null)
                    serializer.Serialize(writer, string.Empty);
                else
                {
                    var stack = value as ItemStack;

                    serializer.Serialize(writer, ItemStack.ToText(stack));
                }
            }

            public TextConverter()
            {

            }
        }

        //Static Utility
        public static List<ItemStack> From(IList<string> IDs)
        {
            string AquireID(string ID) => ID;

            return From(IDs, AquireID);
        }
        public static List<ItemStack> From(IList<ItemTemplate> templates)
        {
            string AquireID(ItemTemplate template) => template == null ? null : template.ID;

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

                if (ID == null)
                {
                    Debug.LogWarning("Null ID found when getting ItemStack from: " + list + ", ignoring");
                    continue;
                }

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

        public static ItemStack From(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            string id;
            uint count;

            if (text[0] == '[')
            {
                text = text.Remove(0, 1);

                var values = text.Split(']');

                count = uint.Parse(values[0]);
                id = values[1];
            }
            else
            {
                id = text;
                count = 1;
            }

            var item = Items.Find(id);

            return new ItemStack(item, count);
        }

        public static string ToText(ItemStack stack)
        {
            if (stack.item == null)
                return string.Empty;
            else
                return '[' + stack.count + ']' + stack.item.ID;
        }
    }
}