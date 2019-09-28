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

using PlayFab;
using PlayFab.ClientModels;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Game
{
    [CreateAssetMenu(menuName = Unit.MenuPath + "Template")]
    public class UnitTemplate : ItemTemplate
    {
        public bool Unlocked
        {
            get
            {
                return Core.Player.Inventory.Contains(this);
            }
        }

        [Space]
        [SerializeField]
        protected UnitClass type;
        public UnitClass Type { get { return type; } }

        [SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        [SerializeField]
        protected float health = 100f;
        public float Health { get { return health; } }

        [SerializeField]
        protected DeploymentData deployment = new DeploymentData(200, 2f);
        public DeploymentData Deployment { get { return deployment; } }
        [Serializable]
        public class DeploymentData
        {
            [SerializeField]
            protected int cost;
            public int Cost
            {
                get
                {
                    return cost;
                }
                set
                {
                    cost = value;
                }
            }

            [SerializeField]
            protected float time;
            public float Time { get { return time; } }

            public DeploymentData(int cost, float time)
            {
                this.cost = cost;
                this.time = time;
            }
        }

        [SerializeField]
        protected AttackData attack = new AttackData(20, 1, 1, 1, Damage.Method.Contact);
        public AttackData Attack { get { return attack; } }
        [Serializable]
        public class AttackData
        {
            [SerializeField]
            protected float damage;
            public float Damage { get { return damage; } }

            [SerializeField]
            protected int range;
            public int Range { get { return range; } }

            [SerializeField]
            protected float distance;
            public float Distance { get { return distance; } }

            [SerializeField]
            protected float duration = 1f;
            public float Duration { get { return duration; } }

            [SerializeField]
            protected Damage.Method method;
            public Damage.Method Method { get { return method; } }

            [SerializeField]
            protected StatusEffectProperty[] statusEffects;
            public StatusEffectProperty[] StatusEffects { get { return statusEffects; } }
            
            public AttackData(float damage, int range, float distance, float duration, Damage.Method method)
            {
                this.damage = damage;
                this.range = range;
                this.distance = distance;
                this.duration = duration;
                this.method = method;
            }
        }

        [Serializable]
        public class StatusEffectProperty
        {
            [SerializeField]
            protected ItemUpgradeType upgrade;
            public ItemUpgradeType Upgrade { get { return upgrade; } }

            [SerializeField]
            protected ProbabilityData probability;
            public ProbabilityData Probability { get { return probability; } }
            [Serializable]
            public class ProbabilityData
            {
                [SerializeField]
                [Range(0f, 100f)]
                protected float min;
                public float Min { get { return min; } }

                [SerializeField]
                [Range(0f, 100f)]
                protected float max;
                public float Max { get { return max; } }

                public float Sample(float rate)
                {
                    return Mathf.Lerp(min, max, rate);
                }
            }

            [SerializeField]
            protected StatusEffectData data;
            public StatusEffectData Data { get { return data; } }
        }

        [SerializeField]
        protected float speed;
        public float Speed { get { return speed; } }
    }
}