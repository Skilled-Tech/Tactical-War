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

        public UnitContextIteratorUI Iterator { get; protected set; }

        public UnitContextInitialUI Initial { get; protected set; }

        public UnitContextUpgradeUI Upgrade { get; protected set; }

        public class Module : UnitsUI.Module
        {
            public UnitContextUI Context { get { return UI.Context; } }

            public UnitTemplate Template { get { return Context.Template; } }

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

            Iterator = Dependancy.Get<UnitContextIteratorUI>(gameObject);

            Initial = Dependancy.Get<UnitContextInitialUI>(gameObject);

            Upgrade = Dependancy.Get<UnitContextUpgradeUI>(gameObject);
        }

        public UnitTemplate Template { get; protected set; }

        public virtual void Show(UnitTemplate template)
        {
            Template = template;

            Character.Set(template);

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
            
        }
    }
}