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

using TMPro;

namespace Game
{
	public class ItemStacksUI : UIElement
	{
        [SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        [SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        public List<ItemStackUITemplate> Elements { get; protected set; }

        public Core Core => Core.Instance;

        public virtual void Init()
        {
            Elements = new List<ItemStackUITemplate>();
        }

        public virtual void Set(IList<ItemStack> requirements)
        {
            Clear();

            if(requirements == null || requirements.Count == 0)
            {
                label.text = Core.Localization.Phrases.Get("No Requirements");
            }
            else
            {
                label.text = Core.Localization.Phrases.Get("Required");

                for (int i = 0; i < requirements.Count; i++)
                {
                    var instance = Create(requirements[i]);

                    Elements.Add(instance);
                }
            }

            label.transform.SetAsLastSibling();

            Show();
        }

        protected virtual ItemStackUITemplate Create(ItemStack requirement)
        {
            var instance = Instantiate(template, transform);

            var script = instance.GetComponent<ItemStackUITemplate>();

            script.Init();
            script.Set(requirement.Item, requirement.Count);

            return script;
        }

        public virtual void Clear()
        {
            for (int i = 0; i < Elements.Count; i++)
                Destroy(Elements[i].gameObject);

            Elements.Clear();
        }
    }
}