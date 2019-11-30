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

using Newtonsoft.Json;

using UnityEngine.Scripting;

namespace Game
{
    [CreateAssetMenu(menuName = ItemTemplate.UpgradesData.MenuPath + "Type")]
    public class ItemUpgradeType : ScriptableObject
    {
        public string ID { get { return name; } }

        public LocalizedPhraseProperty DisplayName { get; protected set; }

        [SerializeField]
        protected IconProperty icon;
        public IconProperty Icon { get { return icon; } }

        public static Core Core { get { return Core.Instance; } }
        public static ItemsCore Items { get { return Core.Items; } }

        public virtual void Init()
        {
            DisplayName = LocalizedPhraseProperty.Create(base.name + " upgrade");
        }

        [Preserve]
        public class Converter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return typeof(ItemUpgradeType).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.Value == null) return null;

                var ID = reader.Value as string;

                var template = Items.Upgrades.Types.Find(ID);

                return template;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var type = value as ItemUpgradeType;

                serializer.Serialize(writer, type.ID);
            }

            public Converter()
            {

            }
        }
    }
}