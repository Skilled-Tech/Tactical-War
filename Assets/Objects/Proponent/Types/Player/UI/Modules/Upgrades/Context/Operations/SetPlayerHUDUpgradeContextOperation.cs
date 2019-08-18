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
	public class SetPlayerHUDUpgradeContextOperation : Operation
	{
		[SerializeField]
        protected ProponentUpgradesContext target;
        public ProponentUpgradesContext Target { get { return target; } }

        public override void Execute()
        {
            FindObjectOfType<PlayerHUDUpgradesContext>().Set(target);
        }
    }
}