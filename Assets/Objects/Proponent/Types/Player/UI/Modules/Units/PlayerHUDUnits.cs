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
	public class PlayerHUDUnits : PlayerHUD.Module
	{
		public PlayerHUDUnitsCreator Creator { get; protected set; }

        public override void Configure(PlayerHUD data)
        {
            base.Configure(data);

            Creator = Dependancy.Get<PlayerHUDUnitsCreator>(gameObject);
        }

        public virtual void SetAge(AgeData age)
        {
            Creator.SetAge(age);
        }
    }
}