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
    public class PlayerProponentAbilitiesSelection : ProponentAbilitiesSelection
    {
        public override IList<AbilityTemplate> List => Player.Ability.Selection.List;

        public static Core Core { get { return Core.Instance; } }

        public static PlayerCore Player { get { return Core.Player; } }
    }
}