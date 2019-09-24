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

namespace Game
{
    public interface IModule<T>
    {
        void Configure(T data);
        void Init();
    }

    public abstract class Module<TData> : MonoBehaviour, IModule<TData>
    {
        protected TData Reference { get; set; }
        public virtual void Configure(TData data)
        {
            this.Reference = data;
        }

        public virtual void Init()
        {

        }
    }

    public static class Modules
    {
        public static List<IModule<TReference>> Configure<TReference>(TReference reference)
            where TReference : Component
        {
            var targets = Dependancy.GetAll<IModule<TReference>>(reference.gameObject);

            for (int i = 0; i < targets.Count; i++)
                targets[i].Configure(reference);

            return targets;
        }

        public static List<IModule<TReference>> Init<TReference>(TReference reference)
            where TReference : Component
        {
            var targets = Dependancy.GetAll<IModule<TReference>>(reference.gameObject);

            for (int i = 0; i < targets.Count; i++)
                targets[i].Init();

            return targets;
        }
    }
}