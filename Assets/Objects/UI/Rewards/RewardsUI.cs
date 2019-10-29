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
	public class RewardsUI : UIElement
	{
        [SerializeField]
        protected TMP_Text title;
        public TMP_Text Title { get { return title; } }

        [SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        [SerializeField]
        protected ItemStackUITemplate itemStack;
        public ItemStackUITemplate ItemStack { get { return itemStack; } }

        [SerializeField]
        protected Button next;
        public Button Next { get { return next; } }

        public Core Core => Core.Instance;

        public virtual void Init()
        {
            next.onClick.AddListener(OnNextClick);
        }

        public List<ItemStack> List { get; protected set; }

        public virtual void Show(string title, IList<ItemTemplate> items)
        {
            var stacks = Game.ItemStack.From(items);

            Show(title, stacks);
        }
        public virtual void Show(string title, IList<ItemStack> stacks)
        {
            if(stacks == null || stacks.Count < 1)
            {
                Finish();
            }
            else
            {
                this.title.text = title;

                this.List = stacks.ToList();

                Set(stacks[0]);

                Show();
            }
        }

        public virtual void Set(ItemStack data)
        {
            label.text = data.Item.DisplayName.Text;

            itemStack.Set(data);
        }

        protected virtual void Increment()
        {
            List.RemoveAt(0);

            if (List.Count > 0)
                Set(List[0]);
            else
                Finish();
        }

        void OnNextClick()
        {
            Increment();
        }

        public event Action OnFinish;
        void Finish()
        {
            Hide();

            if (OnFinish != null) OnFinish();
        }
    }
}