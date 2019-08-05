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
        void Init(T data);
    }

    public class Reference<TData> : MonoBehaviour, IReference<TData>
    {
        protected TData Data { get; set; }
        public virtual void Init(TData data)
        {
            this.Data = data;
        }
    }

    public static class References
    {
        public static List<IReference<TData>> Init<TData>(TData reference)
            where TData : Component
        {
            var targets = Dependancy.GetAll<IReference<TData>>(reference.gameObject);

            for (int i = 0; i < targets.Count; i++)
                targets[i].Init(reference);

            return targets;
        }
    }
}