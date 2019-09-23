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
    public class ProponentUnitsCreatorUI : PlayerUI.Module
    {
        [SerializeField]
        protected GameObject template;
        public GameObject Tempalate { get { return template; } }

        public List<ProponentUnitCreationUITemplate> Elements { get; protected set; }

        public override void Configure(PlayerUI data)
        {
            base.Configure(data);

            Elements = new List<ProponentUnitCreationUITemplate>();
        }

        void OnEnable()
        {
            Proponent.Base.Units.Creator.OnDeployment += OnUnitDeployment;

            Proponent.Base.Units.OnUnitDeath += OnUnitDeath;

            Player.Funds.OnValueChanged += OnFundsChanged;
        }

        void OnUnitDeath(Unit unit, Damage.Result damage)
        {
            UpdateState();
        }

        public virtual void SetSelection(IList<UnitTemplate> list)
        {
            Clear();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                    continue;

                var instance = Create(list[i]);

                instance.Set(Proponent, list[i]);

                Elements.Add(instance);
            }
        }

        void OnFundsChanged()
        {
            UpdateState();
        }
        void OnUnitDeployment(BaseUnitsCreator.Deployment deployment)
        {
            UpdateState();
        }
        void UpdateState()
        {
            for (int i = 0; i < Elements.Count; i++)
                Elements[i].UpdateState();
        }

        public virtual ProponentUnitCreationUITemplate Create(UnitTemplate data)
        {
            var instance = Instantiate(template, transform);

            var script = instance.GetComponent<ProponentUnitCreationUITemplate>();

            return script;
        }

        public virtual void Clear()
        {
            if (Elements == null) return;

            for (int i = 0; i < Elements.Count; i++)
                Destroy(Elements[i].gameObject);

            Elements.Clear();
        }

        void OnDisable()
        {
            Proponent.Base.Units.Creator.OnDeployment -= OnUnitDeployment;

            Proponent.Base.Units.OnUnitDeath -= OnUnitDeath;

            Player.Funds.OnValueChanged -= OnFundsChanged;
        }
    }
}