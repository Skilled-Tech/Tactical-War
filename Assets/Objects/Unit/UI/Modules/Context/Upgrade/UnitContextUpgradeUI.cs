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
        protected RectTransform panel;
        public RectTransform Panel { get { return panel; } }

        [SerializeField]
        protected GameObject template;

        public List<ItemUpgradeTypeUITemplate> Templates { get; protected set; }

        public override void Configure(UnitsUI data)
        {
            base.Configure(data);

            Templates = new List<ItemUpgradeTypeUITemplate>();
        }

        public override void Show()
        {
            base.Show();

            Clear();

            for (int i = 0; i < Template.Upgrades.Applicable.Length; i++)
            {
                var instance = CreateProperty(Template, Template.Upgrades.Applicable[i]);

                Templates.Add(instance);
            }
        }

        protected virtual ItemUpgradeTypeUITemplate CreateProperty(UnitTemplate template, ItemUpgradeType type)
        {
            var instance = Instantiate(this.template, panel);

            var script = instance.GetComponent<ItemUpgradeTypeUITemplate>();

            script.Init();
            script.Set(template, type);

            return script;
        }

        protected virtual void Clear()
        {
            for (int i = 0; i < Templates.Count; i++)
                Destroy(Templates[i].gameObject);

            Templates.Clear();
        }
    }
}