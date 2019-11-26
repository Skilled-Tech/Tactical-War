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

using TMPro;

using RTLTMPro;

namespace Game
{
	public class TMPLocalizedPreset : LocalizationBehaviour.Modifier<TMP_Text>
	{
        public LocalizationPreset Preset => Localization.Presets.Current;

        RTLTextMeshPro RTL;

        protected override void Awake()
        {
            base.Awake();

            RTL = Component as RTLTextMeshPro;
        }

        protected override void Start()
        {
            base.Start();

            if(RTL)
            {
                RTL.Farsi = false;
                RTL.PreserveNumbers = false;

                RTL.UpdateText();
            }
        }

        public override void UpdateState()
        {
            base.UpdateState();

            Component.font = Preset.Font;
        }
    }
}