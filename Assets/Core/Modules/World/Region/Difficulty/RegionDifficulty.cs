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

namespace Game
{
    [CreateAssetMenu(menuName = RegionCore.MenuPath + "Difficulty")]
	public class RegionDifficulty : WorldCore.Element
	{
        public static WorldCore.DifficulyCore Difficulty { get { return World.Difficulty; } }

        public int Index { get; protected set; }

        public override void Configure()
        {
            base.Configure();

            Index = World.Difficulty.IndexOf(this);
        }

        [Preserve]
        public class Converter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return typeof(RegionDifficulty).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.Value == null) return null;

                var index = (int)reader.Value;

                var template = Difficulty.Get(index);

                return template;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var template = value as RegionDifficulty;

                serializer.Serialize(writer, template.Index);
            }

            public Converter()
            {

            }
        }
    }
}