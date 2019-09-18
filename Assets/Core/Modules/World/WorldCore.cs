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
    public class WorldCore : Core.Module
	{
        public const string Name = "world";

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
            public Core Core { get { return Core.Instance; } }

            public WorldCore World { get { return Core.World; } }

            public ScenesCore Scenes { get { return Core.Scenes; } }

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

        public ScenesCore Scenes { get { return Core.Scenes; } }
        public PlayFabCore PlayFab { get { return Core.PlayFab; } }

        public override void Configure()
        {
            base.Configure();

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

            }
            else
            {
                if(module.Data.ContainsKey(Name))
                {
                    var json = module.Data[Name].Value;

                    var jObject = JObject.Parse(json);

                    Load(jObject);
                }
                else
                {

                }
            }
        }

        void Load(JObject jObject)
        {
            var jArray = jObject[nameof(regions)] as JArray;

            for (int i = 0; i < jArray.Count; i++)
            {
                var name = jArray[i]["name"].ToObject<string>();

                var region = Find(name);

                region.Load(jArray[i]);
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
            Current.Set(level);

            SceneManager.LoadScene(Scenes.Level, LoadSceneMode.Single);

            SceneManager.LoadScene(level.Scene.Name, LoadSceneMode.Additive);
        }
    }
}