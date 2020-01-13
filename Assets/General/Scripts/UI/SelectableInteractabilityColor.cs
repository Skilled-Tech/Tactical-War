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
    [RequireComponent(typeof(Graphic))]
	public class SelectableInteractabilityColor : MonoBehaviour
	{
		[SerializeField]
        protected Selectable source;
        public Selectable Source { get { return source; } }

        [SerializeField]
        protected Color on = Color.white;
        public Color On { get { return on; } }

        [SerializeField]
        protected Color off = Color.white;
        public Color Off { get { return off; } }

        public Graphic Graphic { get; protected set; }

        private bool lastInteractableValue;

        protected virtual void Reset()
        {
            source = Dependancy.Get<Selectable>(gameObject, Dependancy.Scope.RecursiveToParents);

            Graphic = GetComponent<Graphic>();

            on = off = (Graphic == null ? Color.white : Graphic.color);
        }

        protected virtual void Awake()
        {
            Graphic = GetComponent<Graphic>();
        }

        protected virtual void Start()
        {
            UpdateState();

            lastInteractableValue = source.interactable;
        }

        protected virtual void Update()
        {
            if (lastInteractableValue != source.interactable)
            {
                UpdateState();
                lastInteractableValue = source.interactable;
            }
        }

        protected virtual void UpdateState()
        {
            Graphic.color = source.interactable ? on : off;
        }
    }
}