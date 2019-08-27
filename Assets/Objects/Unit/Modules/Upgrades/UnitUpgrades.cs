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

        new public UnitData.UpgradesData Data { get; protected set; }

        public UnitData.UpgradesData.TypeData Damage { get; protected set; }

        public UnitData.UpgradesData.TypeData Defense { get; protected set; }

        public UnitData.UpgradesData.TypeData Range { get; protected set; }

        public virtual void Set(UnitData.UpgradesData data)
        {
            this.Data = data;

            if(data == null)
            {

            }
            else
            {
                foreach (var type in data.Types)
                {
                    switch (type.Name) //Temporary Solution, don't laugh
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