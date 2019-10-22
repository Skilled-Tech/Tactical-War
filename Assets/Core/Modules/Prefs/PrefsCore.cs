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
            public override bool Default => false;

            public override string Key => "Need Online Login";
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

                var data = Prefs.Get(Key);

                try
                {
                    return (TData)data;
                }
                catch (InvalidCastException)
                {
                    throw new InvalidCastException("Pref data type mismatch, trying to cast " + data.GetType().Name + " to " + typeof(TData).Name + " of key: " + Key);
                }
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

        public Dictionary<string, object> Dictionary { get; protected set; }

        public const string FileName = "Prefs.json";

        public virtual bool Exists => Core.Data.Exists(FileName);

        public override void Configure()
        {
            base.Configure();

            Dictionary = new Dictionary<string, object>();

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
                    Dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
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

        public object this[string key] => Get(key);
        public virtual object Get(string key)
        {
            if(Contains(key) == false)
            {
                Debug.LogError("No Pref data with key: " + key + " found");
                return null;
            }

            return Dictionary[key];
        }

        public delegate void ChangeDelegate(string key);
        public event ChangeDelegate OnChange;
        protected virtual void Change(string key)
        {
            if (OnChange != null) OnChange(key);

            Save();
        }

        public delegate void SetDelegate(string key, object value);
        public event SetDelegate OnSet;
        public virtual void Set(string key, object value)
        {
            if (Contains(key))
                Dictionary[key] = value;
            else
                Dictionary.Add(key, value);

            if (OnSet != null) OnSet(key, value);

            Change(key);
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