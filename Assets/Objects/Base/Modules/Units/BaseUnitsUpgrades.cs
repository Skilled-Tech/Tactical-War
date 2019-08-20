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
	public class BaseUnitsUpgrades : Base.Module
	{
		public Dictionary<UnitType, ProponentUnitUpgradesContext> Contexts { get; protected set; }

        public BaseUnits Units { get { return Base.Units; } }

        public override void Configure(Base data)
        {
            base.Configure(data);

            Contexts = new Dictionary<UnitType, ProponentUnitUpgradesContext>();
        }

        public override void Init()
        {
            base.Init();

            InitContexts();
        }

        protected virtual void InitContexts()
        {
            var list = Dependancy.GetAll<ProponentUnitUpgradesContext>(Proponent.Upgrades.gameObject);

            foreach (var context in list)
                Contexts.Add(context.Type, context);
        }
    }
}