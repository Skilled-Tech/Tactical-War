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
	public class PrefsCore : Core.Property
    {
        [SerializeField]
        protected VersionProperty version;
        public VersionProperty Version { get { return version; } }
        [Serializable]
        public class VersionProperty : Property<string>
        {
            public override string Default => Application.version;

            public override string Key => "Version";

            public override TryParseDelegate<string> TryParse
            {
                get
                {
                    bool Method(string value, out string result)
                    {
                        result = value;

                        return true;
                    }

                    return Method;
                }
            }

            public override void Init()
            {
                base.Init();

                if (Value != Default)
                    Prefs.NeedOnlineLogin.Value = true;

                if (Exists == false || Value != Default)
                    Value = Default;
            }
        }

        [SerializeField]
        protected NeedOnlineLoginProperty needOnlineLogin;
        public NeedOnlineLoginProperty NeedOnlineLogin { get { return needOnlineLogin; } }
        [Serializable]
        public class NeedOnlineLoginProperty : Property<bool>
        {
            public override bool Default => true;

            public override string Key => "Need Online Login";

            public override TryParseDelegate<bool> TryParse => Boolean.TryParse;
        }

        [Serializable]
        public abstract class Property<TData> : Core.Property
        {
            private TData latest;

            public virtual TData Value
            {
                get
                {
                    if (Exists)
                        return latest;
                    else
                        return Default;
                }
                set
                {
                    Save(value);
                }
            }

            public abstract TData Default { get; }

            public abstract TryParseDelegate<TData> TryParse { get; }

            public abstract string Key { get; }

            public virtual bool Exists => Prefs.Contains(Key);

            public override void Configure()
            {
                base.Configure();

                Update();

                Prefs.OnChange += PrefsChangeCallback;
            }

            protected virtual void PrefsChangeCallback(string key)
            {
                if(key == this.Key)
                    Update();
            }

            public virtual void Update()
            {
                if(Exists)
                    latest = Load();
            }

            public virtual TData Load()
            {
                if(Exists == false)
                    throw new KeyNotFoundException("Trying to load Pref with key: " + Key + " even though it doesn't exist, ignoring");

                return Prefs.Get(Key, TryParse, Default);
            }

            public virtual void Reset()
            {
                Value = Default;
            }

            public virtual void Save()
            {
                Save(Value);
            }
            protected virtual void Save(TData value)
            {
                Prefs.Set(Key, value);
            }

            public PrefsCore Prefs { get { return Core.Prefs; } }
        }

        public const string FileName = "Prefs.json";

        public virtual bool Exists => Core.Data.Exists(FileName);

        public Dictionary<string, string> Dictionary { get; protected set; }

        public string this[string key] => Get(key);

        public override void Configure()
        {
            base.Configure();

            Dictionary = new Dictionary<string, string>();

            Load();

            Register(version);
            Register(needOnlineLogin);
        }

        public virtual void Load()
        {
            if(Exists)
            {
                var json = Core.Data.LoadText(FileName);

                try
                {
                    Dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error reading Prefs Data, resetting, error code: " + e.ToString());
                    Reset();
                }
            }
            else
            {
                Reset();
            }
        }

        public virtual void Reset()
        {
            Dictionary.Clear();

            Save();
        }

        public virtual void Save()
        {
            var json = JsonConvert.SerializeObject(Dictionary, Formatting.Indented);

            Core.Data.Save(FileName, json);
        }

        public virtual bool Contains(string key)
        {
            return Dictionary.ContainsKey(key);
        }

        #region Get
        public virtual string Get(string key)
        {
            if(Contains(key) == false)
                throw new Exception("No Pref data with key: " + key + " found");

            return Dictionary[key];
        }

        public delegate T ParseDelegate<T>(string value);
        public virtual T Get<T>(string key, ParseDelegate<T> parse)
        {
            var value = Get(key);

            return parse(value);
        }

        public delegate bool TryParseDelegate<T>(string value, out T result);
        public virtual T Get<T>(string key, TryParseDelegate<T> tryParse, T defaultvalue)
        {
            if(Contains(key))
            {
                var value = Get(key);

                if (tryParse(value, out var result))
                    return result;

                return defaultvalue;
            }
            else
                return defaultvalue;
        }

        public virtual int GetInt(string key) => Get(key, int.Parse);
        public virtual int GetInt(string key, int defaultvalue) => Get(key, int.TryParse, defaultvalue);

        public virtual float GetFloat(string key) => Get(key, float.Parse);
        public virtual float GetFloat(string key, float defaultvalue) => Get(key, float.TryParse, defaultvalue);

        public virtual T GetEnum<T>(string key)
            where T : struct, IComparable, IConvertible, IFormattable
        {
            T Parse(string value) => (T)Enum.Parse(typeof(T), value);

            return Get(key, Parse);
        }
        public virtual T GetEnum<T>(string key, T defaultvalue)
            where T: struct, IComparable, IConvertible, IFormattable
        {
            return Get(key, Enum.TryParse, defaultvalue);
        }
        #endregion

        public delegate void ChangeDelegate(string key);
        public event ChangeDelegate OnChange;
        protected virtual void Change(string key)
        {
            if (OnChange != null) OnChange(key);

            Save();
        }

        public delegate void SetDelegate(string key, object value);
        public event SetDelegate OnSet;
        public virtual void Set(string key, string value)
        {
            if (Contains(key))
            {
                if(Dictionary[key] == value)
                    return;

                Dictionary[key] = value;
            }
            else
                Dictionary.Add(key, value);

            if (OnSet != null) OnSet(key, value);

            Change(key);
        }
        public virtual void Set(string key, object value)
        {
            Set(key, value.ToString());
        }

        public delegate void RemoveDelegate(string key);
        public event RemoveDelegate OnRemove;
        public virtual void Remove(string key)
        {
            if(Contains(key) == false)
            {
                Debug.LogWarning("Cannot remove non existent Pref key: " + key);
                return;
            }

            Dictionary.Remove(key);

            if (OnRemove != null) OnRemove(key);

            Change(key);
        }
    }
}