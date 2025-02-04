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

using Newtonsoft.Json;

using UnityEngine.Scripting;

namespace Game
{
    [Serializable]
    [JsonObject]
    public struct Currency
    {
        [JsonProperty]
        [JsonConverter(typeof(CurrencyTypeJsonConverter))]
        [SerializeField]
        private CurrencyType type;
        public CurrencyType Type { get { return type; } }

        [JsonProperty]
        [SerializeField]
        private int value;
        public int Value { get { return this.value; } }

        public static Core Core => Core.Instance;

        public override bool Equals(object obj)
        {
            if (obj.GetType() == GetType())
            {
                var target = (Currency)obj;

                if (target.type != type) return false;

                if (target.value != value) return false;

                return true;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return type.GetHashCode() ^ value.GetHashCode();
        }

        public override string ToString()
        {
            if(this.type == CurrencyType.Cents)
            {
                return ToString("USD", value / 100f, "");
            }

            return ToString(type, value, "N0");
        }
        public static string ToString(CurrencyType type, float value, string format)
        {
            return ToString(type.ToString(), value, format);
        }
        public static string ToString(string name, float value, string format)
        {
            name = Core.Localization.Phrases.Get(name);

            return value.ToString(format) + " " + name;
        }

        #region Operators
        public static bool operator ==(Currency one, Currency two)
        {
            return one.Equals(two);
        }
        public static bool operator !=(Currency one, Currency two)
        {
            return !one.Equals(two);
        }

        public static Currency operator *(Currency target, int number)
        {
            return new Currency(target.type, target.Value * number);
        }
        #endregion

        public Currency(CurrencyType type, int value)
        {
            this.type = type;
            this.value = value;
        }
        public Currency(string typeCode, int value) : this(CurrencyCode.To(typeCode), value)
        {

        }
        public Currency(KeyValuePair<string, int> pair) : this(pair.Key, pair.Value)
        {

        }
        public Currency(KeyValuePair<string, uint> pair) : this(pair.Key, (int)pair.Value)
        {

        }
    }

    public enum CurrencyType
    {
        Gold, Jewels, Cents
    }

    [Preserve]
    public class CurrencyTypeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CurrencyType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var type = CurrencyCode.To(reader.Value as string);

            return type;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public CurrencyTypeJsonConverter()
        {

        }
    }

    public static class CurrencyCode
    {
        static readonly ElementData[] list = new ElementData[]
        {
            new ElementData(CurrencyType.Gold, "GD"),
            new ElementData(CurrencyType.Jewels, "JL"),
            new ElementData(CurrencyType.Cents, "RM"),
        };
        [Serializable]
        public struct ElementData
        {
            public CurrencyType type { get; private set; }
            public string Code { get; private set; }

            public ElementData(CurrencyType type, string code)
            {
                this.type = type;
                this.Code = code;
            }
        }

        public static string From(CurrencyType type)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].type == type)
                    return list[i].Code;

            throw new NotImplementedException();
        }

        public static CurrencyType To(string code)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].Code == code)
                    return list[i].type;

            throw new NotImplementedException();
        }
    }
}