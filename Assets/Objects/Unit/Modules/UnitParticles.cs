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
    public class UnitParticles : Unit.Module
    {
        [SerializeField]
        protected ElementData hit;
        public ElementData Hit { get { return hit; } }

        [SerializeField]
        protected ElementData death;
        public ElementData Death { get { return death; } }

        [Serializable]
        public class ElementData
        {
            [SerializeField]
            protected GameObject prefab;
            public GameObject Prefab { get { return prefab; } }

            [SerializeField]
            protected Vector3 offset = Vector3.zero;
            public Vector3 Offset { get { return offset; } }

            [SerializeField]
            protected float scale = 1f;
            public float Scale { get { return scale; } }

            [SerializeField]
            protected bool parent = false;
            public bool Parent { get { return parent; } }
        }

        [SerializeField]
        protected string sortingLayer = "Foreground";
        public string SortingLayer { get { return sortingLayer; } }

        public override void Init()
        {
            Unit.OnTookDamage += OnDamage;

            Unit.OnDeath += OnDeath;
        }

        void OnDamage(Entity damager, float value)
        {
            Spawn(hit);
        }

        void OnDeath(Entity killer)
        {
            Spawn(death);
        }

        GameObject Spawn(ElementData element)
        {
            var instance = Instantiate(element.Prefab, transform.TransformPoint(element.Offset), transform.rotation);

            instance.transform.localScale = Vector3.one * element.Scale;

            if (element.Parent) instance.transform.SetParent(transform, true);

            Tools.SetSortingLayer(instance, sortingLayer);

            return instance;
        }
    }
}