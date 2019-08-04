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
    public interface IReference<T>
    {
        void Init(T reference);
    }

    public class References
    {
        public static List<IReference<TType>> Init<TType>(TType reference)
            where TType : Component
        {
            var targets = Dependancy.GetAll<IReference<TType>>(reference.gameObject);

            for (int i = 0; i < targets.Count; i++)
                targets[i].Init(reference);

            return targets;
        }
    }
}