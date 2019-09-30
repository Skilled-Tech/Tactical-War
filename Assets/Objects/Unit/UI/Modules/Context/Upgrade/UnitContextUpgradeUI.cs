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

using TMPro;

namespace Game
{
    public class UnitContextUpgradeUI : UnitContextUI.Module
    {
        [SerializeField]
        protected TypeData type;
        public TypeData Type { get { return type; } }
        [Serializable]
        public class TypeData
        {
            [SerializeField]
            protected RectTransform panel;
            public RectTransform Panel { get { return panel; } }

            [SerializeField]
            protected GameObject template;
            public GameObject Template { get { return template; } }

            [SerializeField]
            protected ToggleGroup toggleGroup;
            public ToggleGroup ToggleGroup { get { return toggleGroup; } }

            public List<ItemUpgradeTypeUITemplate> Instances { get; protected set; }

            public virtual void Init()
            {
                Instances = new List<ItemUpgradeTypeUITemplate>();
            }

            public virtual void Set(IList<ItemUpgradeType> list)
            {
                Clear();

                for (int i = 0; i < list.Count; i++)
                {
                    var instance = CreateTemplate(list[i]);

                    Instances.Add(instance);
                }

                Instances[0].Toggle.isOn = true;
            }

            protected virtual ItemUpgradeTypeUITemplate CreateTemplate(ItemUpgradeType type)
            {
                var instance = Instantiate(this.template, panel);

                var script = instance.GetComponent<ItemUpgradeTypeUITemplate>();

                script.Init(toggleGroup);
                script.Set(type);

                script.OnSelect += SelectCallback;

                return script;
            }

            public event ItemUpgradeTypeUITemplate.SelectDelegate OnSelect;
            void SelectCallback(ItemUpgradeType type)
            {
                if (OnSelect != null) OnSelect(type);
            }

            protected virtual void Clear()
            {
                for (int i = 0; i < Instances.Count; i++)
                    Destroy(Instances[i].gameObject);

                Instances.Clear();
            }
        }

        [SerializeField]
        protected ItemUpgradePropertyUITemplate property;
        public ItemUpgradePropertyUITemplate Property { get { return property; } }

        public override void Configure(UnitsUI data)
        {
            base.Configure(data);

            
        }

        public override void Init()
        {
            base.Init();

            type.Init();

            type.OnSelect += TypeSelectCallback;

            property.Init();
        }

        void TypeSelectCallback(ItemUpgradeType type)
        {
            property.Set(Template, type);
        }

        public override void Show()
        {
            base.Show();

            type.Set(Template.Upgrades.Applicable);
        }
    }
}