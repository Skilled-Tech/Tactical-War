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
    public class AbilityTemplate : ItemTemplate
    {
        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        [SerializeField]
        protected UsageData usage;
        public UsageData Usage { get { return usage; } }
        [Serializable]
        public class UsageData
        {
            [SerializeField]
            protected float cooldown;
            public float Cooldown { get { return cooldown; } }

            [SerializeField]
            protected int cost;
            public int Cost { get { return cost; } }
        }

        public virtual Ability Spawn(Proponent user)
        {
            var instance = Instantiate(prefab);

            instance.name = prefab.name;

            instance.transform.position = prefab.transform.position;
            instance.transform.rotation = prefab.transform.rotation;

            var script = instance.GetComponent<Ability>();

            script.Configure(user, this);
            script.Init();

            return script;
        }
    }
}