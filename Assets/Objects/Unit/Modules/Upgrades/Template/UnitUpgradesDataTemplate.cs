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
    [CreateAssetMenu(menuName = UnitUpgrades.MenuPath + "Data")]
    public class UnitUpgradesDataTemplate : ScriptableObjectWrapper<UnitUpgradesData>
	{
        public UnitUpgradesDataTemplate()
        {
            data = UnitUpgradesData.Default;
        }
    }
}