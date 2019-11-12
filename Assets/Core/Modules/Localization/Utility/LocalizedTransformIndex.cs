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
	public class LocalizedTransformIndex : LocalizationBehaviour.DataModifer<Transform, int, LocalizedTransformIndex.Element>
	{
        public override int Value
        {
            get => Component.GetSiblingIndex();
            protected set => Component.SetSiblingIndex(value);
        }

        [Serializable]
        public class Element : Element<int> { }
    }
}