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
	public class PlayerHUDUnitsCreator : PlayerHUD.Module
	{
		[SerializeField]
        protected GameObject template;
        public GameObject Tempalate { get { return template; } } 

        public List<PlayerHUDUnitCreationTemplate> Elements { get; protected set; }

        public override void Configure(PlayerHUD data)
        {
            base.Configure(data);

            Elements = new List<PlayerHUDUnitCreationTemplate>();

            Player.Base.Units.Creator.OnDeployment += OnUnitDeployment;
            Player.Base.Units.OnUnitDeath += OnUnitDeath;
            Player.Funds.OnValueChanged += OnFundsChanged;
        }

        void OnUnitDeath(Unit unit, Entity damager)
        {
            UpdateState();
        }

        public virtual void SetAge(Age age)
        {
            Clear();

            for (int i = 0; i < age.Units.Length; i++)
            {
                var instance = Create(age.Units[i]);

                instance.Set(Player, age.Units[i]);

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

        public virtual PlayerHUDUnitCreationTemplate Create(UnitData data)
        {
            var instance = Instantiate(template, transform);

            var script = instance.GetComponent<PlayerHUDUnitCreationTemplate>();

            return script;
        }

        public virtual void Clear()
        {
            if (Elements == null) return;

            for (int i = 0; i < Elements.Count; i++)
                Destroy(Elements[i].gameObject);

            Elements.Clear();
        }
    }
}