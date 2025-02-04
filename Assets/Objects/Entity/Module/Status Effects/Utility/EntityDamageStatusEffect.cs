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
	public class EntityDamageStatusEffect : Entity.Module
	{
        [SerializeField]
        protected StatusEffectData data;
        public StatusEffectData Data { get { return data; } }

        [SerializeField]
        [Range(0f, 100f)]
        protected float probabilty = 50f;
        public float Probabilty { get { return probabilty; } }

        public override void Init()
        {
            base.Init();

            Entity.OnTookDamage += TookDamageCallback;
        }

        void TookDamageCallback(Damage.Result result)
        {
            if(result.Method == Damage.Method.Contact)
            {
                if (StatusEffect.CheckProbability(probabilty))
                {
                    StatusEffect.Afflict(data, result.Source, this.Entity);
                }
            }
        }
    }
}