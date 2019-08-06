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
	public class UnitAttack : Unit.Module
	{
        [SerializeField]
        protected float range = 1f;
        public float Range { get { return range; } }

        [SerializeField]
        protected float damage = 20f;
        public float Damage { get { return damage; } }

        [SerializeField]
        protected float duration = 1f;
        public float Duration { get { return duration; } }

        public virtual void DoDamage(Entity target)
        {
            Unit.DoDamage(target, damage);
        }

        public Entity Target { get; protected set; }
        public virtual Coroutine Do(Entity target)
        {
            this.Target = target;

            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(Procedure());

            return coroutine;
        }

        Coroutine coroutine;
        public bool IsProcessing { get { return coroutine != null; } }
        IEnumerator Procedure()
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.5f));

            DoDamage(Target);

            yield return new WaitForSeconds(duration);

            Target = null;

            coroutine = null;
        }

        public virtual void AttackConnected()
        {
            if (Target != null)
                DoDamage(Target);
        }
    }
}