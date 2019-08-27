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
    public class PlayerProponentUnits : ProponentUnits
    {
        public override IList<UnitTemplate> Selection { get { return Core.Instance.Player.Units.Selection.List; } }

        public override UnitData.UpgradesData GetUpgrade(UnitTemplate unit)
        {
            return Core.Instance.Player.Units.Dictionary[unit].Upgrades;
        }
    }
}