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
        [Space]
        [SerializeField]
        protected Species species;
        public Species Species { get { return species; } }

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
        protected float speed = 1.5f;
        public float Speed { get { return speed; } }

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
            protected float power = 20;
            public float Power { get { return power; } }

            [SerializeField]
            protected int range = 1;
            public int Range { get { return range; } }

            [SerializeField]
            protected float distance = 1f;
            public float Distance { get { return distance; } }

            [SerializeField]
            protected float duration = 1f;
            public float Duration { get { return duration; } }

            [SerializeField]
            protected Damage.Method method = Damage.Method.Contact;
            public Damage.Method Method { get { return method; } }

            [SerializeField]
            protected bool penetrate = false;
            public bool Penetrate { get { return penetrate; } }

            [SerializeField]
            protected SFXData _SFX;
            public SFXData SFX { get { return _SFX; } }
            [Serializable]
            public class SFXData
            {
                [SerializeField]
                protected SFXProperty[] initiate;
                public SFXProperty[] Initiate { get { return initiate; } }

                [SerializeField]
                protected SFXProperty[] connect;
                public SFXProperty[] Connect { get { return connect; } }
            }

            [SerializeField]
            protected StatusEffectProperty[] statusEffects;
            public StatusEffectProperty[] StatusEffects { get { return statusEffects; } }
            
            public AttackData(float damage, int range, float distance, float duration, Damage.Method method)
            {
                this.power = damage;
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
        [Range(0f, 100f)]
        protected float defense = 0;
        public float Defense { get { return defense; } }

        [SerializeField]
        protected int rank = 0;
        public int Rank { get { return rank; } }

        [SerializeField]
        protected IllustrationData illustration;
        public IllustrationData Illustration { get { return illustration; } }
        [Serializable]
        public class IllustrationData
        {
            [SerializeField]
            protected Sprite sprite;
            public Sprite Sprite { get { return sprite; } }

            [SerializeField]
            protected Vector2 offset;
            public Vector2 Offset { get { return offset; } }

            [SerializeField]
            protected float scale = 1f;
            public float Scale { get { return scale; } }
        }

        public override void Init()
        {
            base.Init();

            type.Init();
        }

        protected override void Reset()
        {
            base.Reset();

            visibility = new VisibilityData(false, false);
        }
    }
}