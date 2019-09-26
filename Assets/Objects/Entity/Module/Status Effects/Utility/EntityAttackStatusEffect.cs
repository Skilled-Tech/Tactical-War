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
	public class EntityAttackStatusEffect : Entity.Module
	{
        [SerializeField]
        protected StatusEffectData data;
        public StatusEffectData Data { get { return data; } }

        [SerializeField]
        protected Damage.Method condition = Damage.Method.Contact;
        public Damage.Method Condition { get { return condition; } }

        public override void Init()
        {
            base.Init();

            Entity.OnDoDamage += DoDamageCallback; ;
        }

        void DoDamageCallback(Damage.Result result)
        {
            if(result.Method == condition)
            {
                if (StatusEffect.Afflict(data, result.Target, this.Entity)) //Status Effect Applied
                {

                }
                else
                {

                }
            }
        }
    }
}