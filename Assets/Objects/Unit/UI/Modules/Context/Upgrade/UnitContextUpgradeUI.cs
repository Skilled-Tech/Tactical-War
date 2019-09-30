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
    public class UnitContextUpgradeUI : UnitContextUI.Module
    {
        [SerializeField]
        protected TypeData type;
        public TypeData Type { get { return type; } }
        [Serializable]
        public class TypeData
        {
            [SerializeField]
            protected RectTransform panel;
            public RectTransform Panel { get { return panel; } }

            [SerializeField]
            protected GameObject template;
            public GameObject Template { get { return template; } }

            public List<ItemUpgradeTypeUITemplate> Instances { get; protected set; }

            public virtual void Set(IList<ItemUpgradeType> list)
            {
                Clear();

                for (int i = 0; i < list.Count; i++)
                {
                    var instance = CreateProperty(list[i]);

                    Instances.Add(instance);
                }
            }

            protected virtual ItemUpgradeTypeUITemplate CreateProperty(ItemUpgradeType type)
            {
                var instance = Instantiate(this.template, panel);

                var script = instance.GetComponent<ItemUpgradeTypeUITemplate>();

                script.Init();
                script.Set(type);

                return script;
            }

            protected virtual void Clear()
            {
                for (int i = 0; i < Instances.Count; i++)
                    Destroy(Instances[i].gameObject);

                Instances.Clear();
            }
        }

        [SerializeField]
        protected ItemUpgradePropertyUITemplate property;
        public ItemUpgradePropertyUITemplate Property { get { return property; } }

        public override void Configure(UnitsUI data)
        {
            base.Configure(data);

            
        }

        public override void Show()
        {
            base.Show();

            
        }
    }
}