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
    [CreateAssetMenu(menuName = MenuPath + "Levels")]
	public class LevelsCore : Core.Module
	{
        [SerializeField]
        protected LevelData[] list;
        public LevelData[] List { get { return list; } }

        public int Count { get { return list.Length; } }

        public LevelData this[int index] { get { return list[index]; } }

        public ScenesCore Scenes { get { return Core.Scenes; } }

        public override void Configure()
        {
            base.Configure();

            Current = null;

            LoadData();

            for (int i = 0; i < list.Length; i++)
                list[i].OnChange += OnDataChange;
        }

        void OnDataChange()
        {
            SaveData();
        }

        void SaveData()
        {
            var json = JsonConvert.SerializeObject(list, Formatting.Indented);

            Core.Data.Save(DataPath, json);
        }
        public const string DataPath = "Player/Levels.json";
        void LoadData()
        {
            if(Core.Data.Exists(DataPath))
            {
                var json = Core.Data.LoadText(DataPath);

                var jArray = JArray.Parse(json);

                for (int i = 0; i < list.Length; i++)
                    list[i].Load(jArray[i]);
            }
            else
            {
                for (int i = 0; i < list.Length; i++)
                    list[i].Unlocked = i == 0;
            }
        }

        public LevelData Current { get; protected set; }
        public LevelData Next
        {
            get
            {
                if (Current == null) return null;

                var index = Array.IndexOf(list, Current);

                if (index + 1 >= list.Length) return null;

                return list[index + 1];
            }
        }

        public virtual void Load(LevelData element)
        {
            Current = element;

            SceneManager.LoadScene(Core.Scenes.Level.Name);

            SceneManager.LoadScene(element.Scene, LoadSceneMode.Additive);
        }
        public virtual void Load(int index)
        {
            Load(list[index]);
        }
        
        public virtual void Retry()
        {
            if (Current == null)
                Scenes.Load(SceneManager.GetActiveScene().name);
            else
                Load(Current);
        }

        public virtual void Quit()
        {
            Current = null;
            
            Scenes.Load(Scenes.MainMenu);
        }
    }

    [Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class LevelData
    {
        [SerializeField]
        protected GameScene scene;
        public GameScene Scene { get { return scene; } }

        [JsonProperty(Order = 1)]
        public string name { get { return scene.Name; } }

        [SerializeField]
        protected Sprite icon;
        public Sprite Icon { get { return icon; } }

        [SerializeField]
        protected LevelBackgroundData background;
        public LevelBackgroundData Background { get { return background; } }

        [JsonProperty(Order = 2)]
        [SerializeField]
        protected bool unlocked;
        public bool Unlocked
        {
            get
            {
                return unlocked;
            }
            set
            {
                unlocked = value;

                InvokeChange();
            }
        }

        public virtual void Unlock()
        {
            Unlocked = true;
        }

        public event Action OnChange;
        public virtual void InvokeChange()
        {
            if (OnChange != null) OnChange();
        }

        public virtual void Load(JToken token)
        {
            Unlocked = token[nameof(unlocked)].ToObject<bool>();
        }
    }
}