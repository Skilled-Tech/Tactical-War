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
	public class UnitContextIteratorUI : UnitContextUI.Module
	{
        [SerializeField]
        protected Button previous;
        public Button Previous { get { return previous; } }

        [SerializeField]
        protected Button next;
        public Button Next { get { return next; } }

        public UnitsItemsCore Units { get { return Core.Items.Units; } }

        public override void Configure(UnitsUI data)
        {
            base.Configure(data);

            previous.onClick.AddListener(PreviousCallback);
            next.onClick.AddListener(NextCallback);
        }

        protected virtual void PreviousCallback()
        {
            var index = Units.List.IndexOf(Template);

            var target = GetIteration(index, -1);

            if (target == null)
            {
                //TODO provide some feedback
            }
            else
            {
                Context.Show(target);
            }
        }

        protected virtual void NextCallback()
        {
            var index = Units.List.IndexOf(Template);

            var target = GetIteration(index, 1);

            if (target == null)
            {
                //TODO provide some feedback
            }
            else
            {
                Context.Show(target);
            }
        }

        protected virtual UnitTemplate GetIteration(int index, int increment)
        {
            bool IsValid(UnitTemplate unit)
            {
                if (unit == null) return false;
                if (unit.Unlock.Available == false) return false;

                return true;
            }

            var i = index + increment;
            while (i >= 0 && i < Units.Count)
            {
                if (IsValid(Units[i]))
                    return Units[i];

                i += increment;
            }

            i = index > 0 ? 0 : Units.Count - 1;
            while (i >= 0 && i < Units.Count)
            {
                if (IsValid(Units[i]))
                    return Units[i];

                i += increment;
            }

            return null;
        }
    }
}