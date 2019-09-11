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
	public abstract class ProponentUnits : Proponent.Module
	{
		public abstract IList<UnitTemplate> Selection { get; }

        public abstract ItemUpgradeData GetUpgrade(UnitTemplate unit);
	}
}