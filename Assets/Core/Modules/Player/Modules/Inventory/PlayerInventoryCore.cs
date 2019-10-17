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

using PlayFab;
using PlayFab.ClientModels;

namespace Game
{
    [Serializable]
    public class PlayerInventoryCore : PlayerCore.Module
    {
        [SerializeField]
        protected Funds funds = new Funds(999999);
        public Funds Funds { get { return funds; } }

        public List<ItemData> Items { get; protected set; }
        [Serializable]
        public struct ItemData
        {
            public ItemTemplate Template { get; private set; }

            public string ItemID { get { return Template.CatalogItem.ItemId; } }

            public ItemInstance Instance { get; private set; }

            public ItemData(ItemTemplate template, ItemInstance instance)
            {
                this.Template = template;
                this.Instance = instance;
            }
        }

        public PlayFabCore PlayFab { get { return Core.PlayFab; } }

        public event Action OnUpdate;
        protected virtual void TriggerUpdate()
        {
            if (OnUpdate != null) OnUpdate();
        }

        public override void Configure()
        {
            base.Configure();

            Funds.Configure();
            Items = new List<ItemData>();

            PlayFab.Player.Inventory.OnResponse += OnPlayFabInventoryRetrieved;
        }

        public virtual bool Contains(ItemTemplate template)
        {
            return Contains(template.ID);
        }
        public virtual bool Contains(CatalogItem item)
        {
            return Contains(item.ItemId);
        }
        public virtual bool Contains(string itemID)
        {
            return Contains(itemID, 1);
        }
        public virtual bool Contains(string itemID, uint count)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].ItemID == itemID)
                {
                    if (count <= 1) return true;

                    if (Items[i].Instance.RemainingUses >= count) return true;
                }
            }

            return false;
        }

        public virtual ItemData Find(CatalogItem item)
        {
            if (item == null)
                throw new ArgumentException();

            return Find(item.ItemId);
        }
        public virtual ItemData Find(string itemID)
        {
            for (int i = 0; i < Items.Count; i++)
                if (Items[i].ItemID == itemID)
                    return Items[i];

            throw new ArgumentException();
        }

        public virtual bool CompliesWith(ItemStack requirement)
        {
            if (requirement == null) return true;

            return Contains(requirement.Item.ID, requirement.Count);
        }
        public virtual bool CompliesWith(ItemStack[] requirements)
        {
            if (requirements == null) return true;

            for (int i = 0; i < requirements.Length; i++)
            {
                if (CompliesWith(requirements[i]))
                    continue;
                else
                    return false;
            }

            return true;
        }

        void OnPlayFabInventoryRetrieved(PlayFabPlayerInventoryCore inventory, PlayFabError error)
        {
            if (error == null)
            {
                Load(inventory);
            }
            else
            {

            }
        }

        void Load(PlayFabPlayerInventoryCore inventory)
        {
            funds.Load(inventory.VirtualCurrency);

            Items.Clear();

            for (int i = 0; i < inventory.Items.Count; i++)
            {
                var template = Core.Items.Find(inventory.Items[i].ItemId);

                if (template == null)
                {
                    Debug.LogWarning("No Item Template defined for Inventory Item with ID " + inventory.Items[i].ItemId + ", Skipping");
                    continue;
                }

                var data = new ItemData(template, inventory.Items[i]);

                Items.Add(data);
            }

            TriggerUpdate();
        }
    }
}