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
	public class AbilitySelectionUI : AbilitiesUI.Module
	{
        public AbilitiesSelectionListUI List { get; protected set; }

        public AbilitySelectionDragUI Drag { get; protected set; }

        public override void Configure(AbilitiesUI data)
        {
            base.Configure(data);

            List = Dependancy.Get<AbilitiesSelectionListUI>(gameObject);

            Drag = Dependancy.Get<AbilitySelectionDragUI>(gameObject);
        }
    }
}