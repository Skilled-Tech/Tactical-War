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
	public class PlayerAbilitiesUI : PlayerUI.Element
    {
		[SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        public List<PlayerAbilityUITemplate> Elemenets { get; protected set; }

        public override void Init()
        {
            base.Init();

            Elemenets = new List<PlayerAbilityUITemplate>();

            for (int i = 0; i < Proponent.Abilities.Count; i++)
            {
                var instance = Create(Proponent.Abilities[i]);

                Elemenets.Add(instance);
            }
        }

        public virtual PlayerAbilityUITemplate Create(ProponentAbility ability)
        {
            var instance = Instantiate(template, transform);

            var script = instance.GetComponent<PlayerAbilityUITemplate>();

            script.Init();
            script.Set(ability);
            return script;
        }
    }
}