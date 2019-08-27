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
	public class UnitsUpgradeUI : UnitsUI.Module
	{
        [SerializeField]
        protected RectTransform panel;
        public RectTransform Panel { get { return panel; } }

        [SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        public List<UnitsUpgradePropertyTemplate> Templates { get; protected set; }

        public UnitData Data { get; protected set; }

        public override void Configure(UnitsUI data)
        {
            base.Configure(data);

            Templates = new List<UnitsUpgradePropertyTemplate>();
        }

        public virtual void Set(UnitData unit, UnitUpgradesController upgrades)
        {
            this.Data = unit;

            Clear();

            foreach (var property in upgrades.Types)
            {
                var instance = CreateProperty(property);

                Templates.Add(instance);
            }

            Show();
        }

        protected virtual UnitsUpgradePropertyTemplate CreateProperty(UnitUpgradesController.TypeController property)
        {
            var instance = Instantiate(template, panel);

            var script = instance.GetComponent<UnitsUpgradePropertyTemplate>();

            script.Set(property);

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