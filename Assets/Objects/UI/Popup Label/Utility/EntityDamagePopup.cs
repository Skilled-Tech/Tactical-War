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
	public class EntityDamagePopup : Entity.Module
	{
		[SerializeField]
        protected GameObject prefab;
        public GameObject Prefab { get { return prefab; } }

        public override void Init()
        {
            base.Init();

            Entity.OnTookDamage += OnTookDamage;
        }

        void OnTookDamage(Entity damager, float value)
        {
            var label = Spawn();

            label.Configure();

            label.Text = value.ToString();
        }

        PopupLabel Spawn()
        {
            var instance = Instantiate(prefab, transform.position, Quaternion.identity);

            var label = instance.GetComponent<PopupLabel>();

            return label;
        }
    }
}