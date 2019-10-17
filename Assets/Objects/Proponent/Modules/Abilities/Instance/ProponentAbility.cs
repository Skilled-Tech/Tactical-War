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
        protected Ability selection;
        public Ability Selection { get { return selection; } }

        [SerializeField]
        protected int cost = 500;
        public int Cost { get { return cost; } }

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

                if (Proponent.Energy.Value < cost) return false;

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

            Proponent.Energy.Value -= cost;

            Selection.Activate(Proponent);

            if (OnUse != null) OnUse();
        }
    }
}