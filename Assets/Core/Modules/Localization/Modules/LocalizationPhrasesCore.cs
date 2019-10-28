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
using Newtonsoft.Json.Linq;

namespace Game
{
    [Serializable]
	public class LocalizationPhrasesCore : LocalizationCore.Property
	{
        public Dictionary<string, LocalizedPhrase> Dictionary { get; protected set; }

        public LocalizedPhrase this[string ID] => Dictionary[ID];

        public virtual string FileName => "Phrases Localization.json";
        public virtual string FilePath => Application.streamingAssetsPath + "/Localization/Phrases.json";

        public override void Configure()
        {
            base.Configure();

            Dictionary = new Dictionary<string, LocalizedPhrase>();

            Load();
        }

        void Load()
        {
            var json = File.ReadAllText(FilePath);

            Load(json);
        }
        void Load(string json)
        {
            var jObject = JObject.Parse(json);

            Load(jObject);
        }
        void Load(JObject jObject)
        {
            Dictionary.Clear();

            foreach (var child in jObject.Properties())
            {
                var element = LocalizedPhrase.Load(child);

                Dictionary.Add(child.Name, element);
            }
        }

        public virtual string Get(string ID, string code)
        {
            if (Contains(ID))
            {
                var element = Dictionary[ID];

                if (element.Contains(code))
                    return element[code];
            }

            return "#/#" + ID + "#/#";
        }
        public virtual string Get(string ID, LocalizationType code)
        {
            return Get(ID, LocalizationCode.From(code));
        }

        public virtual bool Contains(string ID)
        {
            return Dictionary.ContainsKey(ID);
        }

        public virtual LocalizedPhrase Get(string ID)
        {
            if (Contains(ID))
                return this[ID];

            return null;
        }
    }

    public class LocalizedPhrase
    {
        public Dictionary<string, string> Dictionary { get; protected set; }

        public string this[string code] => Get(code);
        public string this[LocalizationType type] => Get(type);

        public string Get(string code)
        {
            if(Contains(code))
                return Dictionary[code];

            return "NULL";
        }
        public string Get(LocalizationType type)
        {
            return Get(LocalizationCode.From(type));
        }

        public bool Contains(string code)
        {
            return Dictionary.ContainsKey(code);
        }
        public bool Contains(LocalizationType type)
        {
            return Contains(LocalizationCode.From(type));
        }

        public LocalizedPhrase(Dictionary<string, string> dictionary)
        {
            this.Dictionary = dictionary;
        }

        public static LocalizedPhrase Load(JProperty property)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var child in property.Value.Children<JProperty>())
            {
                dictionary.Add(child.Name, child.Value.ToObject<string>());
            }

            var phrase = new LocalizedPhrase(dictionary);

            return phrase;
        }
    }
}