﻿using System;
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

using Game;

namespace CaptureStudio
{
	public class UnitCapture : PrefabCapture<UnitTemplate>
    {
        public override GameObject ToPrefab(UnitTemplate type) => type.Prefab;
    }
}