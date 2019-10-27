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
    public abstract class ProponentAbilitiesSelection : Proponent.Module
    {
        public abstract IList<AbilityTemplate> List { get; }

        public AbilityTemplate this[int index] => List[index];

        public int Count => List.Count;

        public AbilityTemplate Random
        {
            get
            {
                if (Count == 0) return null;

                return List[UnityEngine.Random.Range(0, Count)];
            }
        }
    }
}