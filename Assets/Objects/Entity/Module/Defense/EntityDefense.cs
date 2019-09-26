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
	public class EntityDefense : Entity.Module
	{
		[SerializeField]
        [Range(0f, 100f)]
        protected float _base = 0f;
        public float Base
        {
            get
            {
                return _base;
            }
        }

        public float Value
        {
            get
            {
                var result = Base;

                for (int i = 0; i < Modifiers.Count; i++)
                    result += Modifiers[i].Value;

                return result;
            }
        }

        public List<IModifier> Modifiers { get; protected set; }
        public interface IModifier
        {
            /// <summary>
            /// Percentage value from 0f to 100f
            /// </summary>
            float Value { get; }
        }

        public override void Configure(Entity reference)
        {
            base.Configure(reference);

            Modifiers = Dependancy.GetAll<IModifier>(Entity.gameObject);
        }

        public virtual float Sample(float damage)
        {
            return Mathf.Lerp(damage, 0f, Value / 100f);
        }
    }
}