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
    public interface IModule<T>
    {
        void Configure(T data);
        void Init();
    }

    public abstract class Module<TData> : MonoBehaviour, IModule<TData>
    {
        protected TData Data { get; set; }
        public virtual void Configure(TData data)
        {
            this.Data = data;
        }

        public virtual void Init()
        {

        }
    }

    public static class Modules
    {
        public static List<IModule<TData>> Configure<TData>(TData data)
            where TData : Component
        {
            var targets = Dependancy.GetAll<IModule<TData>>(data.gameObject);

            for (int i = 0; i < targets.Count; i++)
                targets[i].Configure(data);

            return targets;
        }

        public static List<IModule<TData>> Init<TData>(TData data)
            where TData : Component
        {
            var targets = Dependancy.GetAll<IModule<TData>>(data.gameObject);

            for (int i = 0; i < targets.Count; i++)
                targets[i].Init();

            return targets;
        }
    }
}