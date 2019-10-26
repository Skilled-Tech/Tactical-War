#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

using UnityEditor;
using UnityEditorInternal;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using Game;

namespace CaptureStudio
{
	public class UnitRosterCapture : PrefabCapture<UnitTemplate>
    {
        public override GameObject ToPrefab(UnitTemplate subject) => subject.Prefab;

        protected override string GetPath(UnitTemplate subject)
        {
            var folder = "Roster/" + subject.Species.name + "/" + subject.Type.name + "/";

            var path = CaptureStudio.FormatPath(folder, subject.name);

            return path;
        }
    }
}
#endif