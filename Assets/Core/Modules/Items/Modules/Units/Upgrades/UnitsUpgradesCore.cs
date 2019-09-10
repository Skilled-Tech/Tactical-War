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
	public class ItemsUpgradesCore : ItemsCore.Module
	{
        public const string Key = "Upgrades";

        [SerializeField]
        protected TypesCore types;
        public TypesCore Types { get { return types; } }
        [Serializable]
        public class TypesCore : Module
        {
            [SerializeField]
            protected ItemUpgradeType[] list;
            public ItemUpgradeType[] List { get { return list; } }

            public virtual ItemUpgradeType Find(string name)
            {
                for (int i = 0; i < list.Length; i++)
                    if (list[i].name == name)
                        return list[i];

                return null;
            }
        }

        [SerializeField]
        protected TemplatesCore templates;
        public TemplatesCore Templates { get { return templates; } }
        [Serializable]
        public class TemplatesCore : Module
        {
            [SerializeField]
            protected ItemUpgradesTemplate _default;
            public ItemUpgradesTemplate Default { get { return _default; } }

            [SerializeField]
            protected List<ItemUpgradesTemplate> list;
            public List<ItemUpgradesTemplate> List { get { return list; } }

            public override void Configure()
            {
                base.Configure();

                list = new List<ItemUpgradesTemplate>();

                Core.PlayFab.Title.Data.OnRetrieved += OnTitleDataRetrieved;
            }

            public virtual ItemUpgradesTemplate Find(string name)
            {
                for (int i = 0; i < list.Count; i++)
                    if (list[i].name == name)
                        return list[i];

                return null;
            }

            void OnTitleDataRetrieved(PlayFabTitleDataCore data)
            {
                var jObject = JObject.Parse(data.Value[ItemsUpgradesCore.Key]);

                List.Clear();
                foreach (var item in jObject.Properties())
                {
                    ItemUpgradesTemplate template = null;

                    if (item.Name == nameof(Default))
                        template = Default;
                    else
                        template = ScriptableObject.CreateInstance<ItemUpgradesTemplate>();

                    template.Load(item);

                    List.Add(template);
                }

                var target = list.Last();
            }
        }

        public class Module : ItemsCore.Module
        {
            public ItemsUpgradesCore Upgrades { get { return Items.Upgrades; } }
        }

        public override void Configure()
        {
            base.Configure();

            types.Configure();
            templates.Configure();
        }

        public override void Init()
        {
            base.Init();

            types.Init();
            templates.Init();
        }
    }
}