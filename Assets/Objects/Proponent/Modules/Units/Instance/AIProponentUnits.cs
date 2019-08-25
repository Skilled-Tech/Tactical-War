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
        protected List<UnitData> selection;
        public override IList<UnitData> Selection { get { return selection; } }

        public override UnitUpgradesController GetUpgrade(UnitData unit)
        {
            return null;
        }
    }
}