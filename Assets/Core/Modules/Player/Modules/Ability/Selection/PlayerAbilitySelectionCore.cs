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
    public class PlayerAbilitySelectionCore : PlayerAbilityCore.Module
	{
        [SerializeField]
        protected int max = 2;
        public int Max { get { return max; } }

        #region List
        [SerializeField]
        [JsonProperty(ItemConverterType = typeof(ItemTemplate.Converter))]
        protected AbilityTemplate[] list;
        public AbilityTemplate[] List { get { return list; } }

        public AbilityTemplate this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                if (allowDuplicates == false) ClearAny(value);

                list[index] = value;

                Save();

                OnChange?.Invoke(index, value);
            }
        }
        public delegate void ChangeDelegate(int index, AbilityTemplate target);
        public event ChangeDelegate OnChange;

        public int Count => List.Length;
        #endregion

        [SerializeField]
        protected bool allowDuplicates = false;
        public bool AllowDuplicates { get { return allowDuplicates; } }

        [SerializeField]
        protected ContextCore context;
        public ContextCore Context { get { return context; } }
        [Serializable]
        public class ContextCore : Property
        {
            public AbilityTemplate Template { get; protected set; }

            public int? Slot { get; protected set; }

            public virtual void Start(AbilityTemplate target)
            {
                Template = target;
            }

            public virtual void SetSlot(int? value)
            {
                Slot = value;
            }

            public virtual void Apply()
            {
                if (Slot.HasValue)
                {
                    Ability[Slot.Value] = Template;
                }
                else
                {

                }

                Template = null;
                Slot = null;
            }
        }

        [Serializable]
        public class Property : Core.Property
        {
            public PlayerAbilitySelectionCore Ability => Core.Player.Ability.Selection;
        }

        public override void Configure()
        {
            base.Configure();

            list = new AbilityTemplate[max];

            Player.Inventory.OnUpdate += OnInventoryUpdated;

            Load();

            if (allowDuplicates == false) ClearDuplicates();

            Register(context);
        }

        void OnInventoryUpdated()
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == null)
                    continue;
                else if (Player.Inventory.Contains(list[i]))
                    continue;
                else
                    list[i] = null;
            }

            Save();
        }

        public virtual void ClearAny(AbilityTemplate target)
        {
            if (target == null) return;

            for (int i = 0; i < Count; i++)
                if (this[i] == target) this[i] = null;
        }
        public virtual void ClearDuplicates()
        {
            var hash = new HashSet<AbilityTemplate>();

            for (int i = 0; i < Count; i++)
            {
                if (this[i] == null) continue;

                if (hash.Contains(this[i]))
                {
                    this[i] = null;
                }
                else
                {
                    hash.Add(this[i]);
                }
            }
        }

        public string DataPath { get { return "Player/Ability Selection.json"; } }
        public virtual void Save()
        {
            var json = JsonConvert.SerializeObject(list, Formatting.Indented, new ItemTemplate.Converter());

            Core.Data.Save(DataPath, json);
        }
        public virtual void Load()
        {
            if (Core.Data.Exists(DataPath))
            {
                var json = Core.Data.LoadText(DataPath);

                var array = JArray.Parse(json);

                for (int i = 0; i < max; i++)
                {
                    if (i >= array.Count) continue;

                    list[i] = Core.Items.Abilities.Find(array[i].ToString());
                }
            }
            else
            {

            }
        }

        public virtual void Add(AbilityTemplate template)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] == null)
                {
                    this[i] = template;
                    return;
                }
            }

            this[0] = template;
        }
    }
}