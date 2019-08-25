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
	public class UnitSelectionUI : MonoBehaviour
	{
		[SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        public UnitSelectionUITemplate[] Templates { get; protected set; }

        public Core Core { get { return Core.Instance; } }

        public PlayerUnitsSelectionCore SelectionCore { get { return Core.Player.Units.Selection; } }

        void Start()
        {
            Templates = new UnitSelectionUITemplate[SelectionCore.Max];

            for (int i = 0; i < SelectionCore.Max; i++)
            {
                Templates[i] = CreateTemplate(SelectionCore.List[i], i);

                Templates[i].transform.localEulerAngles = Vector3.forward * Mathf.Lerp(10, -10, i / (SelectionCore.Max - 1f));
            }
        }

        UnitSelectionUITemplate CreateTemplate(UnitData unit, int index)
        {
            var instance = Instantiate(template, transform);

            instance.name = "Slot " + index.ToString();

            var script = instance.GetComponent<UnitSelectionUITemplate>();

            script.Init(index);
            script.Set(unit);

            return script;
        }
    }
}