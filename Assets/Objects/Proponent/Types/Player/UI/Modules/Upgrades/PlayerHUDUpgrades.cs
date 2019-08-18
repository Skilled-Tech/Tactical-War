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
	public class PlayerHUDUpgrades : PlayerHUD.Module
	{
		public PlayerHUDUpgradesContext Context { get; protected set; }

        public override void Configure(PlayerHUD data)
        {
            base.Configure(data);

            Context = Dependancy.Get<PlayerHUDUpgradesContext>(gameObject);
        }

        public virtual void Open()
        {
            Context.Set(Player.Upgrades.Contexts[0]);
        }
    }
}