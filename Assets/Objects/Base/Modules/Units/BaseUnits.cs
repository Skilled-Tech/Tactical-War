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
	public class BaseUnits : Base.Module
	{
		public List<Unit> List { get; protected set; }

        public Unit this[int index]
        {
            get
            {
                return List[index];
            }
        }

        public Unit First
        {
            get
            {
                if (List.Count == 0) return null;

                return List.First();
            }
        }

        public int Count { get { return List.Count; } }

        public event Action<int> OnCountChanged;

        [SerializeField]
        protected int max = 10;
        public int Max { get { return max; } } 

        public BaseUnitsCreator Creator { get; protected set; }

        public override void Configure(Base data)
        {
            base.Configure(data);

            List = new List<Unit>();

            Creator = Dependancy.Get<BaseUnitsCreator>(gameObject);
        }

        public override void Init()
        {
            base.Init();

            Creator.OnSpawn += OnUnitSpawn;
        }

        void OnUnitSpawn(Unit unit)
        {
            unit.Index = Count;

            List.Add(unit);

            unit.OnDeath += (Entity damager)=>UnitDeath(unit, damager);

            if (OnCountChanged != null) OnCountChanged(Count);
        }

        private void Update()
        {
            if(Proponent.name.ToLower().Contains("player") && Input.GetKeyDown(KeyCode.S))
            {
                var unit = List.First();

                unit.Suicide();

                Destroy(unit.gameObject);
            }
        }

        public delegate void UnitDeathDelegate(Unit unit, Entity damager);
        public event UnitDeathDelegate OnUnitDeath;
        void UnitDeath(Unit unit, Entity damager)
        {
            List.Remove(unit);

            UpdateIndexes();

            if (OnCountChanged != null) OnCountChanged(Count);

            if (OnUnitDeath != null) OnUnitDeath(unit, damager);
        }

        void UpdateIndexes()
        {
            for (int i = 0; i < List.Count; i++)
                List[i].Index = i;
        }
    }
}