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
    [CreateAssetMenu(menuName = MenuPath + "Selection")]
	public class PlayerUnitsSelectionCore : PlayerUnitsCore.Module
	{
		[SerializeField]
        protected int max = 4;
        public int Max { get { return max; } }

        [SerializeField]
        protected UnitData[] list;
        public UnitData[] List { get { return list; } }

        public UnitData this[int index]
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

        public UnitData Context;

        public override void Configure()
        {
            base.Configure();

            list = new UnitData[max];

            Load();
        }

        public virtual void Save()
        {
            var array = new string[max];

            for (int i = 0; i < max; i++)
            {
                if (list[i] == null)
                {
                    array[i] = "EMPTY";
                }
                else
                {
                    array[i] = list[i].name;
                }
            }

            var json = JsonConvert.SerializeObject(array, Formatting.Indented);

            Data.Save(DataPath, json);
        }

        public string DataPath { get { return "Player/Units/Selection.json"; } }

        public virtual void Load()
        {
            if (Data.Exists(DataPath))
            {
                var array = JArray.Parse(Data.LoadText(DataPath));

                for (int i = 0; i < max; i++)
                    list[i] = Core.Units.Find(array[i].ToString());
            }
            else
            {
                for (int i = 0; i < max; i++)
                    list[i] = Core.Units.List[i];
            }
        }
    }
}