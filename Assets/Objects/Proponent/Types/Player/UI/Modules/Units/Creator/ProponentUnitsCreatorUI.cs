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

        public override void Init()
        {
            base.Init();

            Set(Proponent.Units.Selection);
        }

        void OnEnable()
        {
            Proponent.Base.Units.Creator.OnDeployment += UnitDeploymentCallback;

            Proponent.Base.Units.OnUnitDeath += UnitDeathCallback;

            Player.Funds.OnValueChanged += FundsChangedCallback;
        }

        protected virtual void Set(IList<UnitTemplate> list)
        {
            Clear();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                    continue;

                var instance = Create(list[i]);

                Elements.Add(instance);
            }
        }

        public virtual void Clear()
        {
            if (Elements == null) return;

            for (int i = 0; i < Elements.Count; i++)
                Destroy(Elements[i].gameObject);

            Elements.Clear();
        }

        public virtual ProponentUnitCreationUITemplate Create(UnitTemplate template)
        {
            var instance = Instantiate(this.template, transform);

            var script = instance.GetComponent<ProponentUnitCreationUITemplate>();

            script.Set(Proponent, template);

            return script;
        }

        void UnitDeathCallback(Unit unit, Damage.Result damage)
        {
            UpdateState();
        }
        void FundsChangedCallback()
        {
            UpdateState();
        }
        void UnitDeploymentCallback(BaseUnitsCreator.Deployment deployment)
        {
            UpdateState();
        }

        void UpdateState()
        {
            for (int i = 0; i < Elements.Count; i++)
                Elements[i].UpdateState();
        }

        void OnDisable()
        {
            Proponent.Base.Units.Creator.OnDeployment -= UnitDeploymentCallback;

            Proponent.Base.Units.OnUnitDeath -= UnitDeathCallback;

            Player.Funds.OnValueChanged -= FundsChangedCallback;
        }
    }
}