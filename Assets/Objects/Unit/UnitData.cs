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
    [CreateAssetMenu]
    public class UnitData : ScriptableObject
    {
        [SerializeField]
        protected UnitType type;
        public UnitType Type { get { return type; } } 

        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        [SerializeField]
        protected Sprite sprite;
        public Sprite Sprite { get { return sprite; } } 

        [SerializeField]
        protected Currency cost;
        public Currency Cost { get { return cost; } }

        [SerializeField]
        protected float deploymentTime;
        public float DeploymentTime { get { return deploymentTime; } } 
    }
}