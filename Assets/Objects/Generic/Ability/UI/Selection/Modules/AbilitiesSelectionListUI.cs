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
	public class AbilitiesSelectionListUI : AbilitiesUI.Module
	{
		[SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        [SerializeField]
        protected float spacing = -10f;
        public float Spacing { get { return spacing; } }

        public AbilitySelectionUITemplate[] Templates { get; protected set; }

        public PlayerAbilitySelectionCore Selection { get { return Core.Player.Ability.Selection; } }

        public override void Init()
        {
            base.Init();

            Templates = new AbilitySelectionUITemplate[Selection.Max];

            var templateWidth = Template.GetComponent<RectTransform>().sizeDelta.x;

            var templateSpace = templateWidth + spacing;

            var totalWidth = templateSpace * (Selection.Max - 1);

            for (int i = 0; i < Selection.Max; i++)
            {
                var rate = i / (Selection.Max - 1f);

                Templates[i] = CreateTemplate(Selection.List[i], i);

                Templates[i].RectTransform.localEulerAngles = Vector3.forward * Mathf.Lerp(10, -10, rate);

                var xPosition = Mathf.Lerp(-totalWidth / 2f, totalWidth / 2f, rate);

                Templates[i].RectTransform.anchoredPosition = new Vector2(xPosition, 0f);
            }
        }

        AbilitySelectionUITemplate CreateTemplate(AbilityTemplate unit, int index)
        {
            var instance = Instantiate(template, transform);

            instance.name = "Slot " + index.ToString();

            var script = instance.GetComponent<AbilitySelectionUITemplate>();

            script.Init(index);
            script.Set(unit);

            return script;
        }
    }
}