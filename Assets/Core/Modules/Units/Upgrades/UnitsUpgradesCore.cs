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
	public class UnitsUpgradesCore : UnitsCore.Module
	{
        public const string Key = "Upgrades";

        [SerializeField]
        protected TypesCore types;
        public TypesCore Types { get { return types; } }
        [Serializable]
        public class TypesCore : Module
        {
            [SerializeField]
            protected UnitUpgradeType[] list;
            public UnitUpgradeType[] List { get { return list; } }

            public virtual UnitUpgradeType Find(string name)
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
            protected UnitUpgradesTemplate _default;
            public UnitUpgradesTemplate Default { get { return _default; } }

            [SerializeField]
            protected List<UnitUpgradesTemplate> list;
            public List<UnitUpgradesTemplate> List { get { return list; } }

            public override void Configure()
            {
                base.Configure();

                list = new List<UnitUpgradesTemplate>();

                Core.PlayFab.Title.Data.OnRetrieved += OnTitleDataRetrieved;
            }

            public virtual UnitUpgradesTemplate Find(string name)
            {
                for (int i = 0; i < list.Count; i++)
                    if (list[i].name == name)
                        return list[i];

                return null;
            }

            void OnTitleDataRetrieved(PlayFabTitleDataCore data)
            {
                var jObject = JObject.Parse(data.Value[UnitsUpgradesCore.Key]);

                List.Clear();
                foreach (var item in jObject.Properties())
                {
                    UnitUpgradesTemplate template = null;

                    if (item.Name == nameof(Default))
                        template = Default;
                    else
                        template = ScriptableObject.CreateInstance<UnitUpgradesTemplate>();

                    template.Load(item);

                    List.Add(template);
                }

                var target = list.Last();
            }
        }

        public class Module : UnitsCore.Module
        {
            public UnitsUpgradesCore Upgrades { get { return Units.Upgrades; } }
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