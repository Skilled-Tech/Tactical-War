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
	public class UnitContextUI : UnitsUI.Module
	{
        public UnitCharacterUI Character { get; protected set; }

        public UnitContextInitialUI Initial { get; protected set; }

        public UnitContextUpgradeUI Upgrade { get; protected set; }

        public class Module : UnitsUI.Module
        {
            public UnitContextUI Context { get { return UI.Context; } }

            public DataElement Data { get { return Context.Data; } }

            public override void Show()
            {
                base.Show();

                UpdateState();
            }

            public virtual void UpdateState()
            {

            }
        }

        public override void Configure(UnitsUI data)
        {
            base.Configure(data);

            Character = Dependancy.Get<UnitCharacterUI>(gameObject);

            Initial = Dependancy.Get<UnitContextInitialUI>(gameObject);

            Upgrade = Dependancy.Get<UnitContextUpgradeUI>(gameObject);
        }

        public DataElement Data;
        public class DataElement
        {
            public UnitTemplate Asset { get; protected set; }

            public UnitData Instance { get; protected set; }

            public DataElement(UnitTemplate unit)
            {
                this.Asset = unit;

                this.Instance = Core.Instance.Player.Units.Dictionary[unit];
            }
        }

        public virtual void Set(UnitTemplate unit)
        {
            if(Data != null)
                Data.Instance.OnChange -= UpdateState;

            Data = new DataElement(unit);

            Data.Instance.OnChange += UpdateState;

            Character.Set(unit);

            Initial.Show();
            Upgrade.Hide();

            Show();
        }

        void UpdateState()
        {
            Initial.UpdateState();

            Upgrade.UpdateState();
        }

        void OnDestroy()
        {
            if(Data != null)
                Data.Instance.OnChange -= UpdateState;
        }
    }
}