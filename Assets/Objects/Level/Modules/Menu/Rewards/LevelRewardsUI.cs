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
	public class LevelRewardsUI : LevelMenu.Element
	{
		[SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        [SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        [SerializeField]
        protected TMP_Text ammount;
        public TMP_Text Ammount { get { return ammount; } }

        [SerializeField]
        protected Button button;
        public Button Button { get { return button; } }

        public override void Configure(LevelMenu data)
        {
            base.Configure(data);

            button.onClick.AddListener(ClickAction);
        }

        public List<ItemRequirementData> List { get; protected set; }

        public virtual void Show(IList<ItemRequirementData> list)
        {
            if(list == null || list.Count < 1)
            {
                Debug.LogWarning("Can't Display Rewards, Invalid Arguments");
                return;
            }

            this.List = list.ToList();

            Set(list[0]);

            Show();
        }

        public virtual void Set(ItemRequirementData data)
        {
            label.text = data.Item.name;

            ammount.text = "x" + data.Count.ToString();

            icon.sprite = data.Item.Icon;
        }

        void ClickAction()
        {
            List.RemoveAt(0);

            if(List.Count > 0)
            {
                Set(List[0]);
            }
            else
            {
                Finish();
            }
        }

        public event Action OnFinish;
        void Finish()
        {
            if (OnFinish != null) OnFinish();
        }
    }
}