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
	public class ProjectileDamage : Projectile.Module
	{
		[SerializeField]
        protected float _value;
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value < 0f) value = 0f;

                _value = value;
            }
        }

        public override void Init()
        {
            base.Init();

            Projectile.OnCollision += OnCollision;
        }

        void OnCollision(Collision2D obj)
        {
            var entity = obj.gameObject.GetComponent<Entity>();

            if(entity == null)
            {

            }
            else if(Owner is Base && entity is Base) //Very Crude, Don't judge me
            {

            }
            else
            {
                Owner.DoDamage(entity, Value);
            }
        }
    }
}