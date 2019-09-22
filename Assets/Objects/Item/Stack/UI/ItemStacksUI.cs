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
	public class ItemStacksUI : UIElement
	{
        [SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        [SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        public List<ItemStackUITemplate> Elements { get; protected set; }

        public virtual void Init()
        {
            Elements = new List<ItemStackUITemplate>();
        }

        public virtual void Set(ItemStack[] requirements)
        {
            Clear();

            if(requirements == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);

                if (requirements.Length == 0)
                {
                    label.text = "No Materials Required";
                }
                else
                {
                    label.text = "Required";

                    for (int i = 0; i < requirements.Length; i++)
                    {
                        var instance = Create(requirements[i]);

                        Elements.Add(instance);
                    }
                }
            }

            label.transform.SetAsLastSibling();
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