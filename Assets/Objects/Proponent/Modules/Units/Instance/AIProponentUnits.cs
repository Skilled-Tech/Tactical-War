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
	public class AIProponentUnits : ProponentUnits
    {
		[SerializeField]
        protected List<UnitTemplate> selection;
        public override IList<UnitTemplate> Selection { get { return selection; } }

        public override UnitData.UpgradesData GetUpgrade(UnitTemplate unit)
        {
            return null;
        }
    }
}