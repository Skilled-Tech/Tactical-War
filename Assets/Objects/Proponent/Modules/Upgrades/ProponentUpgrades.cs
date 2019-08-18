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
	public class ProponentUpgrades : Proponent.Module
	{
        public List<ProponentUpgradesContext> Contexts { get; protected set; }

		public class Module : Module<ProponentUpgrades>
        {
            public ProponentUpgrades Upgrades { get { return Data; } }

            public Proponent Proponent { get { return Upgrades.Proponent; } }
        }

        public override void Configure(Proponent data)
        {
            base.Configure(data);

            Contexts = Dependancy.GetAll<ProponentUpgradesContext>(gameObject);

            Modules.Configure(this);
        }

        public override void Init()
        {
            base.Init();

            Modules.Init(this);
        }
    }
}