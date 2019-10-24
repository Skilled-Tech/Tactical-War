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
	public class EntityHealthActivation : Entity.Module
	{
        [SerializeField]
        protected List<GameObject> list;
        public List<GameObject> List { get { return list; } }

        public override void Init()
        {
            base.Init();

            UpdateState();

            Entity.Health.OnValueChanged += OnHealthChange;
        }

        void OnHealthChange(float value)
        {
            UpdateState();
        }

        void UpdateState()
        {
            var rate = Mathf.Lerp(0, list.Count - 1, Entity.Health.Rate);

            var index = Mathf.CeilToInt(rate);

            for (int i = 0; i < list.Count; i++)
                list[i].SetActive(i == index);
        }
    }
}