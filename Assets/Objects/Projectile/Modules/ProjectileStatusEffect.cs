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
	public class ProjectileStatusEffect : Projectile.Module
	{
		[SerializeField]
        protected StatusEffectData data;
        public StatusEffectData Data { get { return data; } }

        public override void Init()
        {
            base.Init();

            Projectile.OnHit += HitCallback;
        }

        void HitCallback(Collider2D collider)
        {
            var entity = collider.gameObject.GetComponent<Entity>();

            if (entity == null)
            {

            }
            else if (Owner is Base && entity is Base) //Very Crude, Don't judge me
            {

            }
            else
            {
                StatusEffect.Afflict(data, entity, Owner);
            }
        }
    }
}