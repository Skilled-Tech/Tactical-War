﻿using System;
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

        public Core Core => Core.Instance;

        public FaderUI Fader => Core.UI.Fader;

        private void Start()
        {
            values = new List<LocalizationType>();
            foreach (LocalizationType item in Enum.GetValues(typeof(LocalizationType)))
                values.Add(item);
        }

        public override void Execute()
        {
            Core.SceneAcessor.StartCoroutine(Procedure());

            IEnumerator Procedure()
            {
                yield return Fader.To(1f, 0.2f);

                Localization.Progress();

                yield return new WaitForSeconds(0.2f);

                yield return Fader.To(0f, 0.2f);
            }
        }
    }
}