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
	public class UnitDefenseUpgradeProperty : UnitUpgrades.Property
	{
        protected override void UpdateState()
        {
            base.UpdateState();

            Unit.Defense.Value = UpgradeProperty.Current == null ? 0f : UpgradeProperty.Current.Percentage;
        }
    }
}