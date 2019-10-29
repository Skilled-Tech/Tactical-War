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
	public class ProgressLocalizationOperation : Operation
	{
        public LocalizationCore Localization => Core.Instance.Localization;

        List<LocalizationType> values;

        private void Start()
        {
            values = new List<LocalizationType>();
            foreach (LocalizationType item in Enum.GetValues(typeof(LocalizationType)))
                values.Add(item);
        }

        public override void Execute()
        {
            var index = values.IndexOf(Localization.Target);

            if (index + 1 < values.Count)
                Localization.Target = values[index + 1];
            else
                Localization.Target = values[0];
        }
    }
}