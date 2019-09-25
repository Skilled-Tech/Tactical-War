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
	public class EntityTimeScale : Entity.Module
	{
        //0% to 100%
		public virtual float Value
        {
            get
            {
                var result = 0f;

                for (int i = 0; i < List.Count; i++)
                    result += List[i].Value;

                return result;
            }
        }

        //0 to 1
        public virtual float Rate
        {
            get
            {
                return Value / 100f;
            }
        }

        public List<IModifer> List { get; protected set; }

        public interface IModifer
        {
            /// <summary>
            /// To be implemeted as a value from 0% to 100% or beyond if you're into things not working :)
            /// </summary>
            float Value { get; }
        }

        public override void Init()
        {
            base.Init();

            List = Dependancy.GetAll<IModifer>(gameObject);
        }
    }
}