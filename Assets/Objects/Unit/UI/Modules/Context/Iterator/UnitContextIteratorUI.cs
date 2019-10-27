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

            var target = index == 0 ? Units.List.Last() : Units.List[index - 1];

            Context.Show(target);
        }

        protected virtual void NextCallback()
        {
            var index = Units.List.IndexOf(Template);

            var target = index + 1 == Units.Count ? Units.List.First() : Units.List[index + 1];

            Context.Show(target);
        }
    }
}