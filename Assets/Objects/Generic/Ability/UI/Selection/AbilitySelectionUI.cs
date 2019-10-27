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
	public class AbilitySelectionUI : UnitsUI.Module
	{
        public UnitSelectionListUI List { get; protected set; }

        public UnitSelectionDragUI Drag { get; protected set; }

        public override void Configure(UnitsUI data)
        {
            base.Configure(data);

            List = Dependancy.Get<UnitSelectionListUI>(gameObject);

            Drag = Dependancy.Get<UnitSelectionDragUI>(gameObject);
        }
    }
}