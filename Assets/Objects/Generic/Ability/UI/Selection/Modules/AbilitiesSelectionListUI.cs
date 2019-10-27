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

        public PlayerAbilitySelectionCore SelectionCore { get { return Core.Player.Ability.Selection; } }

        public override void Init()
        {
            base.Init();

            Templates = new AbilitySelectionUITemplate[SelectionCore.Max];

            var templateWidth = Template.GetComponent<RectTransform>().sizeDelta.x;

            var templateSpace = templateWidth + spacing;

            var totalWidth = templateSpace * (SelectionCore.Max - 1);

            for (int i = 0; i < SelectionCore.Max; i++)
            {
                var rate = i / (SelectionCore.Max - 1f);

                Templates[i] = CreateTemplate(SelectionCore.List[i], i);

                Templates[i].RectTransform.localEulerAngles = Vector3.forward * Mathf.Lerp(10, -10, rate);

                var xPosition = Mathf.Lerp(-totalWidth / 2f, totalWidth / 2f, rate);

                Templates[i].RectTransform.anchoredPosition = new Vector2(xPosition, 0f);
            }

            UI.Selection.Drag.OnDragEnd += OnTemplateDragEnd;
        }

        void OnTemplateDragEnd()
        {
            for (int i = 0; i < Templates.Length; i++)
                Templates[i].OnTemplateDragEnd();
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