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
	public class ProponentUpgradesContext : ProponentUpgrades.Module
	{
        public List<ProponentUpgradeProperty> Properties { get; protected set; }

        public virtual ProponentUpgradeProperty Find(UpgradeType type)
        {
            for (int i = 0; i < Properties.Count; i++)
                if (Properties[i].Type == type)
                    return Properties[i];

            return null;
        }

        public override void Configure(ProponentUpgrades data)
        {
            base.Configure(data);

            Properties = Dependancy.GetAll<ProponentUpgradeProperty>(gameObject);
        }

        public override void Init()
        {
            base.Init();

            foreach (var property in Properties)
            {
                property.OnUpgrade += ()=> PropertyUpgraded(property);
            }
        }

        public event Action<ProponentUpgradeProperty> OnPropertyUpgraded;
        protected virtual void PropertyUpgraded(ProponentUpgradeProperty property)
        {
            if (OnPropertyUpgraded != null) OnPropertyUpgraded(property);
        }
    }
}