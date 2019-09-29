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
    public class ProponentUnitsUI : PlayerUI.Module
    {
        public ProponentUnitsCreatorUI Creator { get; protected set; }

        public override void Configure(PlayerUI data)
        {
            base.Configure(data);

            Creator = Dependancy.Get<ProponentUnitsCreatorUI>(gameObject);
        }
    }
}