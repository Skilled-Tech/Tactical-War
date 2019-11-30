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

using PlayFab.ClientModels;

namespace Game
{
    [Serializable]
    public class ItemsUpgradesCore : ItemsCore.Module
    {
        public const string Key = "upgrades";

        [SerializeField]
        protected TypesCore types;
        public TypesCore Types { get { return types; } }
        [Serializable]
        public class TypesCore : Module
        {
            [SerializeField]
            protected ItemUpgradeType[] list;
            public ItemUpgradeType[] List { get { return list; } }

            [SerializeField]
            protected CommonData common;
            public CommonData Common { get { return common; } }
            [Serializable]
            public class CommonData
            {
                [SerializeField]
                protected ItemUpgradeType power;
                public ItemUpgradeType Power { get { return power; } }

                [SerializeField]
                protected ItemUpgradeType defense;
                public ItemUpgradeType Defense { get { return defense; } }

                [SerializeField]
                protected ItemUpgradeType range;
                public ItemUpgradeType Range { get { return range; } }
            }

            public virtual ItemUpgradeType Find(string name)
            {
                for (int i = 0; i < list.Length; i++)
                    if (list[i].name == name)
                        return list[i];

                return null;
            }
            public virtual ItemUpgradeType Find(JToken token)
            {
                return Find(token.ToObject<string>());
            }

            public override void Init()
            {
                base.Init();

                for (int i = 0; i < list.Length; i++)
                {
                    list[i].Init();
                }
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

            public ItemUpgradesTemplate[] List { get; protected set; }

            public override void Configure()
            {
                base.Configure();

                Core.PlayFab.Title.Data.OnRetrieved += OnTitleDataRetrieved;
            }

            public virtual ItemUpgradesTemplate Find(string name)
            {
                for (int i = 0; i < List.Length; i++)
                    if (List[i].name == name)
                        return List[i];

                return null;
            }

            void OnTitleDataRetrieved(PlayFabTitleDataCore data)
            {
                var jArray = JArray.Parse(data.Value[ItemsUpgradesCore.Key]);

                List = new ItemUpgradesTemplate[jArray.Count];

                for (int i = 0; i < jArray.Count; i++)
                {
                    ItemUpgradesTemplate template = null;

                    if (jArray[i][ItemUpgradesTemplate.ID].ToObject<string>() == nameof(Default))
                        template = Default;
                    else
                        template = ScriptableObject.CreateInstance<ItemUpgradesTemplate>();

                    template.Load(jArray[i].ToString());

                    List[i] = template;
                }

                var target = List.Last();
            }
        }

        public class Module : ItemsCore.Module
        {
            public ItemsUpgradesCore Upgrades { get { return Items.Upgrades; } }
        }

        public override void Configure()
        {
            base.Configure();

            Register(types);
            Register(templates);
        }

        public virtual bool IsUpgradable(CatalogItem item)
        {
            if (item == null)
                throw new NullReferenceException();

            if (item.CustomData == null) return false;

            if (item.Tags == null) return false;

            if (item.Tags.Contains("upgradable"))
                return true;
            else
                return false;
        }
    }
}