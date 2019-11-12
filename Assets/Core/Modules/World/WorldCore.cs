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
    public class WorldCore : Core.Property
	{
        public const string MenuPath = Core.Module.MenuPath + "World/";

        public const string ID = "world";

        #region Regions
        [SerializeField]
        protected RegionCore[] regions;
        public RegionCore[] Regions { get { return regions; } }

        public int Size { get { return regions.Length; } }

        public RegionCore this[int index] { get { return regions[index]; } }

        public int IndexOf(RegionCore region)
        {
            for (int i = 0; i < regions.Length; i++)
                if (regions[i] == region)
                    return i;

            throw new ArgumentException();
        }

        public RegionCore Find(string name)
        {
            for (int i = 0; i < regions.Length; i++)
                if (regions[i].name == name)
                    return regions[i];

            return null;
        }
        #endregion

        [Serializable]
        public class Element : Core.Property
        {

        }
        public class Module : Core.Module
        {
            new public const string MenuPath = WorldCore.MenuPath + "Modules/";

            public static WorldCore World { get { return Core.World; } }

            public static ScenesCore Scenes { get { return Core.Scenes; } }
        }

        [SerializeField]
        protected DifficulyElement difficulty;
        public DifficulyElement Difficulty { get { return difficulty; } }
        [Serializable]
        public class DifficulyElement : Element
        {
            [SerializeField]
            protected RegionDifficulty[] list;
            public RegionDifficulty[] List { get { return list; } }

            public RegionDifficulty this[int index] { get { return list[index]; } }

            public int Count { get { return list.Length; } }

            public RegionDifficulty First { get { return list[0]; } }
            public RegionDifficulty Last { get { return list.Last(); } }

            public virtual int IndexOf(RegionDifficulty element)
            {
                return Array.IndexOf(list, element);
            }

            public virtual bool Contains(RegionDifficulty element)
            {
                return list.Contains(element);
            }

            public override void Configure()
            {
                base.Configure();

                for (int i = 0; i < list.Length; i++)
                    Register(list[i]);
            }
        }

        public ScenesCore Scenes { get { return Core.Scenes; } }
        public PlayFabCore PlayFab { get { return Core.PlayFab; } }

        public override void Configure()
        {
            base.Configure();

            Register(difficulty);

            for (int i = 0; i < regions.Length; i++)
            {
                Register(regions[i]);
            }

            PlayFab.Player.ReadonlyData.OnRetrieved += OnPlayerReadOnlyDataRetrieved;
        }

        void OnPlayerReadOnlyDataRetrieved(PlayFabPlayerReadOnlyData module)
        {
            var dictionary = new Dictionary<string, JToken>();

            if (module.Data != null && module.Data.ContainsKey(ID))
            {
                var json = module.Data[ID].Value;

                var jObject = JObject.Parse(json);

                var array = jObject[nameof(regions)] as JArray;

                for (int i = 0; i < array.Count; i++)
                {
                    var name = array[i]["name"].ToObject<string>();

                    dictionary.Add(name, array[i]);
                }
            }

            for (int i = 0; i < regions.Length; i++)
            {
                if (dictionary.ContainsKey(regions[i].name))
                    regions[i].Parse(dictionary[regions[i].name]);
                else
                    regions[i].ApplyDefaults();
            }
        }

        public SelectionData Current { get; protected set; }
        [Serializable]
        public class SelectionData
        {
            public LevelCore Level { get; set; }

            public RegionDifficulty Difficulty { get; set; }

            public SelectionData()
            {

            }
            public SelectionData(LevelCore level, RegionDifficulty difficulty) : this()
            {
                this.Level = level;
                this.Difficulty = difficulty;
            }
        }
        public virtual void Load(SelectionData selection) => Load(selection.Level, selection.Difficulty);
        public virtual void Load(LevelCore level) => Load(level, level.Region.Progress.Difficulty);
        public virtual void Load(LevelCore level, RegionDifficulty difficulty)
        {
            Current = new SelectionData(level, difficulty);

            Scenes.Load.All(level.Scene, Scenes.Level);
        }
    }
}