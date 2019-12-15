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

using UnityEngine.Networking;

namespace Game
{
    [Serializable]
	public class LocalizationPhrasesCore : LocalizationCore.Property
	{
        public Dictionary<string, LocalizedPhrase> Dictionary { get; protected set; }

        public LocalizedPhrase this[string ID] => Dictionary[ID];

        public virtual string DirectoryPath => Application.streamingAssetsPath + "/Localization/Phrases/";

        public override void Configure()
        {
            base.Configure();

            Dictionary = new Dictionary<string, LocalizedPhrase>(StringComparer.OrdinalIgnoreCase);

            Load();
        }

        void Load()
        {
            var files = Resources.LoadAll<TextAsset>("Localization/Phrases/");

            for (int i = 0; i < files.Length; i++)
            {
                Load(files[i].text);
            }
        }
        void Load(string json)
        {
            var jObject = JObject.Parse(json);

            Load(jObject);
        }
        void Load(JObject jObject)
        {
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

            return "*" + ID + "*";
        }
        public virtual string Get(string ID, LocalizationType code)
        {
            return Get(ID, LocalizationCode.From(code));
        }
        public virtual string Get(string ID)
        {
            return Get(ID, Localization.Target);
        }

        public virtual bool Contains(string ID)
        {
            return Dictionary.ContainsKey(ID);
        }

        public virtual LocalizedPhrase Find(string ID)
        {
            if (Contains(ID))
                return this[ID];

            return null;
        }
    }

    public static class LocalizationBehaviour
    {
        public abstract class Base : MonoBehaviour
        {
            public Core Core => Core.Instance;

            public LocalizationCore Localization => Core.Localization;

            protected virtual void Awake()
            {

            }

            protected virtual void Start()
            {
                Localization.OnTargetChange += TargetChangeCallback;
            }

            protected virtual void TargetChangeCallback(LocalizationType type)
            {

            }

            protected virtual void OnDestroy()
            {
                Localization.OnTargetChange -= TargetChangeCallback;
            }
        }

        public abstract class Modifier<TComponent> : Base
            where TComponent : Component
        {
            public TComponent Component { get; protected set; }

            protected virtual void Reset()
            {
                Component = GetComponent<TComponent>();
            }

            protected override void Awake()
            {
                base.Awake();

                Component = GetComponent<TComponent>();
            }

            protected override void Start()
            {
                base.Start();

                UpdateState();
            }

            protected override void TargetChangeCallback(LocalizationType type)
            {
                base.TargetChangeCallback(type);

                UpdateState();
            }

            public virtual void UpdateState()
            {

            }
        }

        public abstract class PhraseLabel<TComponent> : Modifier<TComponent>
            where TComponent : Component
        {
            [SerializeField]
            string _ID;
            public string ID
            {
                get
                {
                    return _ID;
                }
                set
                {
                    _ID = value;
                }
            }

            public abstract string Text { get; protected set; }

            public LocalizedPhrase Phrase { get; protected set; }

            protected override void Reset()
            {
                base.Reset();

                ID = Text.ToLower();
            }

            protected override void Start()
            {
                Phrase = Localization.Phrases.Find(ID);

                base.Start();
            }

            public override void UpdateState()
            {
                base.UpdateState();

                Text = Retrieve();
            }

            public virtual string Retrieve()
            {
                if (Phrase == null) return "*" + ID + "*";

                return Phrase[Localization.Target];
            }
        }

        public abstract class DataModifer<TComponent, TData, TElement> : Modifier<TComponent>
            where TComponent : Component
            where TElement : DataModifer<TComponent, TData, TElement>.Element<TData>
        {
            public TData Default { get; protected set; }

            public abstract TData Value { get; protected set; }

            [SerializeField]
            protected TElement[] elements;
            public TElement[] Elements { get { return elements; } }

            [Serializable]
            public class Element<TValue>
            {
                [SerializeField]
                protected LocalizationType localization;
                public LocalizationType Localization { get { return localization; } }

                [SerializeField]
                protected TValue value;
                public TValue Value { get { return value; } }

                public Element()
                {

                }
                public Element(LocalizationType localization, TValue value)
                {
                    this.localization = localization;
                    this.value = value;
                }
            }

            protected override void Start()
            {
                Default = Value;

                base.Start();
            }

            public virtual TData From(LocalizationType localization)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i].Localization == localization)
                        return elements[i].Value;
                }

                return Default;
            }

            public override void UpdateState()
            {
                base.UpdateState();

                Value = From(Localization.Target);
            }
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

        public override string ToString()
        {
            var text = "";

            foreach (var pair in Dictionary)
            {
                text += pair.ToString();

                text += Environment.NewLine;
            }

            return text;
        }

        public static LocalizedPhrase Load(JProperty property)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var child in property.Value.Children<JProperty>())
            {
                dictionary.Add(child.Name, child.Value.ToObject<string>());
            }

            var phrase = new LocalizedPhrase(dictionary);

            return phrase;
        }
    }

    [Serializable]
    public class LocalizedPhraseProperty
    {
        [SerializeField]
        protected string _ID;
        public string ID => _ID;

        public string Text
        {
            get
            {
                if (Phrase == null) return "#" + ID + "#";

                return Phrase[Localization.Target];
            }
        }

        public LocalizedPhrase Phrase { get; protected set; }

        public LocalizationCore Localization => Core.Instance.Localization;

        public virtual void Init(string ID)
        {
            _ID = ID;

            Init();
        }
        public virtual void Init()
        {
            Phrase = Localization.Phrases.Find(ID);

            if (Phrase == null)
            {
                Debug.LogWarning("No localization found for " + ID + ", please add");
            }
        }

        public static LocalizedPhraseProperty Create(string ID)
        {
            var data = new LocalizedPhraseProperty();

            data.Init(ID);

            return data;
        }
    }
}