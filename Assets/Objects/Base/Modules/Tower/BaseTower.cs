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

using UnityEngine.EventSystems;

namespace Game
{
	public class BaseTower : Base.Module
	{
        public BaseTowerSlots Slots { get; protected set; }

        public override void Configure(Base data)
        {
            base.Configure(data);

            Slots = Dependancy.Get<BaseTowerSlots>(gameObject);
        }
    }
}