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
	public class ItemsListUI : UIElement
	{
        [SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        public List<ItemUITemplate> Elements { get; protected set; }

        public Core Core { get { return Core.Instance; } }

        public virtual void Init()
        {
            Elements = new List<ItemUITemplate>();
        }

        public virtual void Set(IList<ItemTemplate> list)
        {
            Clear();

            for (int i = 0; i < list.Count; i++)
            {
                var element = CreateTemplate(list[i]);

                Elements.Add(element);
            }
        }

        public virtual void Clear()
        {
            for (int i = 0; i < Elements.Count; i++)
                Destroy(Elements[i].gameObject);

            Elements.Clear();
        }

        protected virtual ItemUITemplate CreateTemplate(ItemTemplate template)
        {
            var instance = Instantiate(this.template, transform);

            var script = instance.GetComponent<ItemUITemplate>();

            script.Init();
            script.Set(template);

            script.OnClick += ElemenetClickCallback; ;

            return script;
        }

        public delegate void SelectionDelegate(ItemTemplate template);
        public event SelectionDelegate OnSelection;
        void ElemenetClickCallback(ItemUITemplate template)
        {
            if (OnSelection != null) OnSelection(template.Template);
        }
    }
}