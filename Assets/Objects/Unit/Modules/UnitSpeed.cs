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
	public class UnitSpeed : Unit.Module, EntityTimeScale.IModifer
	{
        public float Initial => Template.MovementSpeed;

        public float Percentage
        {
            get
            {
                Upgrades.GetElements(Core.Items.Upgrades.Types.Common.Speed, out var template, out var data);

                if (data == null) return 0f;

                return template.Percentage.Calculate(data.Index);
            }
        }
        public float Multiplier => 1f + (Percentage / 100f);

        public float Value
        {
            get
            {
                return Initial * Multiplier * Unit.TimeScale.Rate;
            }
        }
    }
}