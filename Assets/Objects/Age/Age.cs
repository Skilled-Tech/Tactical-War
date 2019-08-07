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
	public class Age : ScriptableObject
	{
        [SerializeField]
        protected UnitData[] units;
        public UnitData[] Units { get { return units; } }

        [SerializeField]
        protected GameScene scene;
        public GameScene Scene { get { return scene; } } 

        [SerializeField]
        protected Currency cost;
        public Currency Cost { get { return cost; } }
    }

    [Serializable]
    public class AgeUpgrades<TData> : MonoBehaviour
    {
        [SerializeField]
        protected TData[] list;
        public TData[] List { get { return list; } } 

        public TData Current { get; protected set; }

        public Level Level { get { return Level.Instance; } }
        public LevelAges Ages { get { return Level.Ages; } }

        public virtual void Configure(Proponent proponent)
        {
            Current = Get(proponent.Age.Value);

            proponent.Age.OnValueChanged += OnAgeChanged;
        }

        protected virtual void OnAgeChanged(Age age)
        {
            Current = Get(age);
        }

        public virtual TData Get(Age age)
        {
            var index = Ages.IndexOf(age);

            return list[index];
        }
    }
}