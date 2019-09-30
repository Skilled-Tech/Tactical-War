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

            [SerializeField]
            protected GameObject template;

            [SerializeField]
            protected ToggleGroup toggleGroup;

            [SerializeField]
            protected float spacing = -10f;

            [SerializeField]
            protected bool autoSelectFirst = true;

            public List<ItemUpgradeTypeUITemplate> Instances { get; protected set; }

            public void Init()
            {
                Instances = new List<ItemUpgradeTypeUITemplate>();
            }
            
            public void Set(IList<ItemUpgradeType> list)
            {
                Clear();

                toggleGroup.allowSwitchOff = true;

                var templateWidth = template.GetComponent<RectTransform>().sizeDelta.x;

                var templateSpace = templateWidth + spacing;

                var totalWidth = templateSpace * (list.Count - 1);

                for (int i = 0; i < list.Count; i++)
                {
                    var instance = CreateTemplate(list[i]);

                    var rate = i / (list.Count - 1f);

                    var position = instance.RectTransform.anchoredPosition;
                    {
                        position.x = Mathf.Lerp(-totalWidth / 2f, totalWidth / 2f, rate);
                    }
                    instance.RectTransform.anchoredPosition = position;

                    Instances.Add(instance);
                }

                if (autoSelectFirst)
                    Instances[0].Toggle.isOn = true;
            }

            private ItemUpgradeTypeUITemplate CreateTemplate(ItemUpgradeType type)
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
                toggleGroup.allowSwitchOff = false;

                if (OnSelect != null) OnSelect(type);
            }

            private void Clear()
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

        public override void Show()
        {
            base.Show();

            property.Hide();

            type.Set(Template.Upgrades.Applicable);
        }

        void TypeSelectCallback(ItemUpgradeType selection)
        {
            property.Show();

            property.Set(Template, selection);
        }
    }
}