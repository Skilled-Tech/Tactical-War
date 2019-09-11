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

using PlayFab.ClientModels;

namespace Game
{
	public class InventoryUI : UIElement
	{
        [SerializeField]
        protected GameObject template;
        public GameObject Template { get { return template; } }

        [SerializeField]
        protected RectTransform panel;
        public RectTransform Panel { get { return panel; } }

        public List<ItemInstanceUITemplate> Elements { get; protected set; }

        public Core Core { get { return Core.Instance; } }
        public PlayerCore Player { get { return Core.Player; } }
        public PlayerInventoryCore Inventory { get { return Player.Inventory; } }

        void Awake()
        {
            Elements = new List<ItemInstanceUITemplate>();

            UpdateState();

            Inventory.OnRetrieved += OnInventoryRetrieved;
        }

        void OnInventoryRetrieved(PlayerInventoryCore inventory)
        {
            UpdateState();
        }

        void UpdateState()
        {
            Clear();

            for (int i = 0; i < Inventory.Items.Count; i++)
            {
                var template = Core.Items.Find(Inventory.Items[i].ItemId);

                if (template is UnitTemplate) continue;

                var instance = CreateInstance(Inventory.Items[i], template);

                Elements.Add(instance);
            }
        }

        void Clear()
        {
            for (int i = 0; i < Elements.Count; i++)
                Destroy(Elements[i].gameObject);

            Elements.Clear();
        }

        ItemInstanceUITemplate CreateInstance(ItemInstance instance, ItemTemplate template)
        {
            var gameObject = Instantiate(this.template, panel);

            var script = gameObject.GetComponent<ItemInstanceUITemplate>();

            script.Init();
            script.Set(instance, template);

            return script;
        }
    }
}