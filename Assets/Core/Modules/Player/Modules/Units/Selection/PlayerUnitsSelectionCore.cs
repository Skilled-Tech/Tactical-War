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
    public class PlayerUnitsSelectionCore : PlayerUnitsCore.Module
	{
		[SerializeField]
        protected int max = 4;
        public int Max { get { return max; } }

        [SerializeField]
        [JsonProperty(ItemConverterType = typeof(ItemTemplate.Converter))]
        protected UnitTemplate[] list;
        public UnitTemplate[] List { get { return list; } }

        public UnitTemplate this[int index]
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

        public UnitTemplate Context;

        public override void Configure()
        {
            base.Configure();

            list = new UnitTemplate[max];

            Player.Inventory.OnUpdate += OnInventoryUpdated;

            Load();
        }

        void OnInventoryUpdated()
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == null)
                    continue;
                else if (list[i].Unlocked)
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

        public string DataPath { get { return "Player/Units Selection.json"; } }

        public virtual void Load()
        {
            if (Core.Data.Exists(DataPath))
            {
                var json = Core.Data.LoadText(DataPath);

                var array = JArray.Parse(json);

                for (int i = 0; i < max; i++)
                {
                    if (i >= array.Count) continue;

                    list[i] = Core.Items.Units.Find(array[i].ToString());
                }
            }
            else
            {
                
            }
        }
    }
}