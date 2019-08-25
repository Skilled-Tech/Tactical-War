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
    public class UnitUpgrades : Unit.Module
    {
        public const string MenuPath = Unit.MenuPath + "Upgrades/";

        new public UnitUpgradesController Controller { get; protected set; }

        public UnitUpgradesController.TypeController Damage { get; protected set; }

        public UnitUpgradesController.TypeController Defense { get; protected set; }

        public UnitUpgradesController.TypeController Range { get; protected set; }

        public virtual void Set(UnitUpgradesController controller)
        {
            this.Controller = controller;

            if(controller == null)
            {

            }
            else
            {
                foreach (var type in controller.Types)
                {
                    switch (type.name) //Temporary Solution, don't laugh
                    {
                        case nameof(Damage):
                            Damage = type;
                            break;
                        case nameof(Defense):
                            Defense = type;
                            break;
                        case nameof(Range):
                            Range = type;
                            break;
                    }
                }
            }
        }
    }
}