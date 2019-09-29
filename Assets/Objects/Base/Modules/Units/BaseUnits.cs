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
                return List.First();
            }
        }

        public int Count { get { return List.Count; } }

        public event Action<int> OnCountChanged;
        protected virtual void TriggerCountChanged()
        {
            if (OnCountChanged != null) OnCountChanged(Count);
        }

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

            Creator.OnSpawn += SpawnCallback;
        }

        void SpawnCallback(Unit unit)
        {
            unit.Index = Count;

            List.Add(unit);

            unit.OnDeath += (Damage.Result result)=> DealthCallback(unit, result);

            TriggerCountChanged();
        }
        
        public delegate void UnitDeathDelegate(Unit unit, Damage.Result damage);
        public event UnitDeathDelegate OnUnitDeath;
        void DealthCallback(Unit unit, Damage.Result result)
        {
            List.Remove(unit);

            UpdateIndexes();

            TriggerCountChanged();

            if (OnUnitDeath != null) OnUnitDeath(unit, result);
        }

        void UpdateIndexes()
        {
            for (int i = 0; i < List.Count; i++)
                List[i].Index = i;
        }
    }
}