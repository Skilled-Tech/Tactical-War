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

namespace Game
{
	public class PlayerHUDUpgradesContext : PlayerHUD.Module
	{
        [SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        public UpgradePropertyTemplate[] Properties { get; protected set; }

        public PlayerHUDUpgrades Upgrades { get { return HUD.Upgrades; } }

        public override void Configure(PlayerHUD data)
        {
            base.Configure(data);

            Properties = new UpgradePropertyTemplate[] { };
        }

        new public ProponentUpgradesContext Target;
        public virtual void Set(ProponentUpgradesContext context)
        {
            this.Target = context;

            for (int i = 0; i < Properties.Length; i++)
                Destroy(Properties[i].gameObject);

            Properties = new UpgradePropertyTemplate[context.Properties.Count];

            for (int i = 0; i < context.Properties.Count; i++)
                Properties[i] = Create(context.Properties[i]);

            Show();

            Upgrades.Show();

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        UpgradePropertyTemplate Create(ProponentUpgradeProperty property)
        {
            var instance = Instantiate(template, transform);

            var component = instance.GetComponent<UpgradePropertyTemplate>();

            component.Set(property);

            return component;
        }
    }
}