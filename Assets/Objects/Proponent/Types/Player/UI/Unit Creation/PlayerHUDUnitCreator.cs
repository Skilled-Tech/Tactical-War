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
	public class PlayerHUDUnitCreator : MonoBehaviour
	{
		[SerializeField]
        protected GameObject template;
        public GameObject Tempalate { get { return template; } } 

        public List<PlayerHUDUnitCreationTemplate> Elements { get; protected set; }

        protected virtual void Awake()
        {
            Elements = new List<PlayerHUDUnitCreationTemplate>();
        }

        public virtual void Set(Age age)
        {
            Clear();

            for (int i = 0; i < age.Units.Length; i++)
            {
                var instance = Create(age.Units[i]);

                instance.Set(age.Units[i]);

                Elements.Add(instance);
            }
        }

        public virtual PlayerHUDUnitCreationTemplate Create(UnitData data)
        {
            var instance = Instantiate(template, transform);

            var script = instance.GetComponent<PlayerHUDUnitCreationTemplate>();

            return script;
        }

        public virtual void Clear()
        {
            if (Elements == null) return;

            for (int i = 0; i < Elements.Count; i++)
                Destroy(Elements[i].gameObject);

            Elements.Clear();
        }
    }
}