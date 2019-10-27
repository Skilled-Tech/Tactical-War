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
	public class ProponentAbility : ProponentAbilities.Module
	{
        public AbilityTemplate Template { get; protected set; }
        public virtual void Set(AbilityTemplate reference)
        {
            this.Template = reference;
        }

        public class Module : Module<ProponentAbility>
        {
            public ProponentAbility Ability => Reference;
        }

        public ProponentAbilityCooldown Cooldown { get; protected set; }

        public virtual bool CanUse
        {
            get
            {
                if (Proponent.Energy.Value < Template.Usage.Cost) return false;

                if (Cooldown.Timer > 0f) return false;

                return true;
            }
        }

        public override void Configure(ProponentAbilities reference)
        {
            base.Configure(reference);

            Cooldown = Dependancy.Get<ProponentAbilityCooldown>(gameObject);

            Modules.Configure(this);
        }

        public override void Init()
        {
            base.Init();

            Modules.Init(this);
        }

        public virtual void Use()
        {
            Proponent.Energy.Value -= Template.Usage.Cost;

            var instance = Template.Spawn(Proponent);

            instance.transform.rotation = Proponent.transform.rotation;

            Cooldown.Begin();
        }

        //Static Utility
        public static ProponentAbility Create(ProponentAbilities abilities, AbilityTemplate template)
        {
            var instance = new GameObject(template.name);

            instance.transform.SetParent(abilities.transform);

            var script = instance.AddComponent<ProponentAbility>();

            script.Set(template);

            var cooldown = ProponentAbilityCooldown.Create(script);

            return script;
        }
    }
}