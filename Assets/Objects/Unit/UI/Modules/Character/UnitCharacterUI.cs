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

namespace Game
{
	public class UnitCharacterUI : UnitsUI.Module
	{
        [SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        [SerializeField]
        protected TMP_Text type;
        public TMP_Text Type { get { return type; } }

        [SerializeField]
        protected RectTransform slot;
        public RectTransform Slot { get { return slot; } }

        public UnitTemplate Data { get; protected set; }

        public virtual void Set(UnitTemplate data)
        {
            label.text = data.name;

            type.text = data.Type.name;

            if(Model == null)
            {

            }
            else
            {
                Destroy(Model.gameObject);
            }

            Model = InstantiateModel(data);
        }

        public Unit Model { get; protected set; }

        protected virtual Unit InstantiateModel(UnitTemplate data)
        {
            var instance = Instantiate(data.Prefab, slot);

            var unit = instance.GetComponent<Unit>();
            unit.enabled = false;

            var controller = instance.GetComponent<UnitController>();
            controller.enabled = false;

            var canvas = Dependancy.Get<Canvas>(instance);
            canvas.gameObject.SetActive(false);

            return unit;
        }
    }
}