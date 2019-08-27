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
	public class UnitsContextUI : UnitsUI.Module
	{
        [SerializeField]
        protected TMP_Text description;
        public TMP_Text Description { get { return description; } }

        [SerializeField]
        protected Button upgrade;
        public Button Upgrade { get { return upgrade; } }

        public UnitData Data { get; protected set; }

        protected virtual void Awake()
        {
            upgrade.onClick.AddListener(UpgradeClick);
        }

        public virtual void Set(UnitData data)
        {
            this.Data = data;

            description.text = data.Description;

            Show();
        }

        void UpgradeClick()
        {
            Hide();

            UI.Upgrade.Set(Data, Core.Player.Units.Upgrades.Find(Data));
        }
	}
}