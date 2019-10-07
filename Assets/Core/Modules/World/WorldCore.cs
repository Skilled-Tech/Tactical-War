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
using Newtonsoft.Json.Linq;

namespace Game
{
    [Serializable]
    public class WorldCore : Core.Module
	{
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
        public class Module : Core.Module
        {

        }
        public class Element : ScriptableObject, Core.IModule
        {
            public static Core Core { get { return Core.Instance; } }

            public static WorldCore World { get { return Core.World; } }

            public static ScenesCore Scenes { get { return Core.Scenes; } }

            public virtual void Configure()
            {

            }

            public virtual void Register(Core.IModule module)
            {
                module.Configure();

                OnInit += module.Init;
            }

            public event Action OnInit;
            public virtual void Init()
            {
                if (OnInit != null) OnInit();
            }
        }

        [SerializeField]
        protected DifficulyCore difficulty;
        public DifficulyCore Difficulty { get { return difficulty; } }
        [Serializable]
        public class DifficulyCore : Module
        {
            [SerializeField]
            protected RegionDifficulty[] list;
            public RegionDifficulty[] List { get { return list; } }

            public virtual RegionDifficulty Get(int index)
            {
                if (index < 0 || index + 1 > list.Length)
                {
                    Debug.LogWarning("no difficulty defined at index: " + index);
                    return null;
                }

                return list[index];
            }

            public virtual int IndexOf(RegionDifficulty element)
            {
                return Array.IndexOf(list, element);
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
            if(module.Data == null)
            {
                ApplyDefaults();
            }
            else
            {
                if(module.Data.ContainsKey(ID))
                {
                    var json = module.Data[ID].Value;

                    var jObject = JObject.Parse(json);

                    Parse(jObject);
                }
                else
                {
                    ApplyDefaults();
                }
            }
        }

        void Parse(JObject jObject)
        {
            var jArray = jObject[nameof(regions)] as JArray;

            var dictionary = jArray.ToDictionary(x => x["name"].ToObject<string>());

            for (int i = 0; i < regions.Length; i++)
            {
                if (dictionary.ContainsKey(regions[i].name))
                {
                    regions[i].Parse(dictionary[regions[i].name]);
                }
                else
                {
                    if (i == 0 || regions[i].Previous.Finished)
                        regions[i].Unlock();
                    else
                        regions[i].Lock();
                }
            }
        }
        void ApplyDefaults()
        {
            for (int i = 0; i < regions.Length; i++)
            {
                if (i == 0)
                    regions[i].Unlock();
                else
                    regions[i].Lock();
            }
        }

        public CurrentData Current { get; protected set; }
        [Serializable]
        public class CurrentData
        {
            public LevelCore Level { get; set; }

            public RegionCore Region { get { return Level.Region; } }

            public virtual void Set(LevelCore level)
            {
                this.Level = level;
            }

            public CurrentData(LevelCore level)
            {
                Set(level);
            }
        }
        public virtual void Load(RegionCore region, LevelCore level)
        {
            Current = new CurrentData(level);

            SceneManager.LoadScene(Scenes.Level, LoadSceneMode.Single);

            SceneManager.LoadScene(level.Scene.Name, LoadSceneMode.Additive);
        }
    }
}