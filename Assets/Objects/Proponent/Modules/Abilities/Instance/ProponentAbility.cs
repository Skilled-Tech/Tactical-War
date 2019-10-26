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
    public class ProponentAbility : Proponent.Module
    {
        [SerializeField]
        protected AbilityTemplate template;
        public AbilityTemplate Template { get { return template; } }

        public int Cost => template.Usage.Cost;

        public ProponentAbilityCooldown Cooldown { get; protected set; }

        public class Module : Module<ProponentAbility>
        {
            public ProponentAbility Ability { get { return Reference; } }

            public Proponent Proponent { get { return Ability.Proponent; } }
        }

        public virtual bool CanUse
        {
            get
            {
                if (Cooldown.Timer > 0f) return false;

                if (Proponent.Energy.Value < Cost) return false;

                return true;
            }
        }

        public override void Configure(Proponent data)
        {
            base.Configure(data);

            Cooldown = Dependancy.Get<ProponentAbilityCooldown>(gameObject);

            Modules.Configure(this);
        }

        public override void Init()
        {
            base.Init();

            Modules.Init(this);
        }

        public event Action OnUse;
        public virtual void Use()
        {
            if (!CanUse)
                throw new Exception(Proponent.name + " Trying to use ability when they aren't able to");

            Proponent.Energy.Value -= Cost;

            var instance = template.Spawn(Proponent); //TODO

            if (OnUse != null) OnUse();
        }
    }
}