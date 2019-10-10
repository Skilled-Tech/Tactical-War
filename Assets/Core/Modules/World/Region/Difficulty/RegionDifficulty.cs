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
	public class RegionDifficulty : WorldCore.Module
	{
        public static WorldCore.DifficulyElement Difficulty { get { return World.Difficulty; } }

        public int Index { get; protected set; }
        public int ID { get { return Index + 1; } }

        public virtual bool IsFirst { get { return Index == 0; } }
        public virtual bool IsLast { get { return Index >= Difficulty.Count - 1; } }

        public virtual RegionDifficulty Previous
        {
            get
            {
                if (IsFirst) return null;

                return Difficulty[Index - 1];
            }
        }
        public virtual RegionDifficulty Next
        {
            get
            {
                if (IsLast) return null;

                return Difficulty[Index + 1];
            }
        }

        public override void Configure()
        {
            base.Configure();

            Index = World.Difficulty.IndexOf(this);
        }

        public static bool operator > (RegionDifficulty one, RegionDifficulty two)
        {
            return one.ID > two.ID;
        }
        public static bool operator <(RegionDifficulty one, RegionDifficulty two)
        {
            return one.ID < two.ID;
        }
        public static bool operator >=(RegionDifficulty one, RegionDifficulty two)
        {
            return one.ID >= two.ID;
        }
        public static bool operator <=(RegionDifficulty one, RegionDifficulty two)
        {
            return one.ID <= two.ID;
        }

        [Preserve]
        public class Converter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                Debug.Log(objectType.FullName);
                return typeof(RegionDifficulty).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.Value == null) return null;

                var index = (int)(Int64)reader.Value;

                var template = Difficulty[index - 1];

                return template;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var template = value as RegionDifficulty;

                serializer.Serialize(writer, template.ID);
            }

            public Converter()
            {

            }
        }
    }
}