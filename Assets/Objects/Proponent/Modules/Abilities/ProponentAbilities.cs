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
	public class ProponentAbilities : Proponent.Module
	{
        public List<ProponentAbility> Elements { get; protected set; }
        public int Count => Elements.Count;
        public ProponentAbility this[int index] => Elements[index];
        
        public class Module : Module<ProponentAbilities>
        {
            public ProponentAbilities Abilities => Reference;

            public Proponent Proponent => Abilities.Proponent;
        }

        public ProponentAbilitiesSelection Selection { get; protected set; }

        public Core Core => Core.Instance;

        public override void Configure(Proponent reference)
        {
            base.Configure(reference);

            Elements = new List<ProponentAbility>();

            Selection = Dependancy.Get<ProponentAbilitiesSelection>(gameObject);
        }

        public override void Init()
        {
            base.Init();

            for (int i = 0; i < Selection.Count; i++)
            {
                if (Selection[i] == null) continue;

                var instance = ProponentAbility.Create(this, Selection[i]);

                Elements.Add(instance);

                Modules.Setup(instance, this);
            }
        }
    }
}