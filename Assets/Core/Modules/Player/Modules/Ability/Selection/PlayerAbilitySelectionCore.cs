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
                list[index] = value;

                Save();
            }
        }

        public AbilityTemplate Context;

        public override void Configure()
        {
            base.Configure();

            list = new AbilityTemplate[max];

            Player.Inventory.OnUpdate += OnInventoryUpdated;

            Load();
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

        public virtual void Save()
        {
            var json = JsonConvert.SerializeObject(list, Formatting.Indented, new ItemTemplate.Converter());

            Core.Data.Save(DataPath, json);

            Core.Data.Save(DataPath, json);
        }

        public string DataPath { get { return "Player/Ability Selection.json"; } }

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
    }
}