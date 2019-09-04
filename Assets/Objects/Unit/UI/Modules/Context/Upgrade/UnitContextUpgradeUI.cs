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
        public GameObject Template { get { return template; } }

        public List<UnitsUpgradePropertyTemplate> Templates { get; protected set; }

        public override void Configure(UnitsUI data)
        {
            base.Configure(data);

            Templates = new List<UnitsUpgradePropertyTemplate>();
        }

        public override void Show()
        {
            base.Show();

            Clear();

            foreach (var property in Core.Player.Units.Dictionary[Data.Template].Upgrades.Types)
            {
                var instance = CreateProperty(property);

                Templates.Add(instance);
            }
        }

        protected virtual UnitsUpgradePropertyTemplate CreateProperty(UnitData.UpgradesData.TypeData property)
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