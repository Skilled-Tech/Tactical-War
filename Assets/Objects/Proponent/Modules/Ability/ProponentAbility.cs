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
        protected Ability _selection;
        public Ability Selection
        {
            get
            {
                return _selection;
            }
            set
            {
                _selection = value;

                if (OnSelectionChanged != null) OnSelectionChanged(Selection);
            }
        }
        public event Action<Ability> OnSelectionChanged;

        public ProponentAbilityCooldown Cooldown { get; protected set; }

        public virtual bool CanUse
        {
            get
            {
                return Cooldown.Timer == 0f && Proponent.Energy.Value > Selection.Cost;
            }
        }

        public override void Configure(Proponent data)
        {
            base.Configure(data);

            Cooldown = Dependancy.Get<ProponentAbilityCooldown>(gameObject);
        }

        public event Action OnUse;
        public virtual void Use()
        {
            if (!CanUse)
                throw new Exception(Proponent.name + " Trying to use ability when they aren't able to");

            Selection.Activate(Proponent);

            if (OnUse != null) OnUse();
        }
    }
}