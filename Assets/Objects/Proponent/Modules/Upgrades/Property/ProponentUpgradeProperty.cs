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
    public class ProponentUpgradeProperty : ProponentUpgrades.Module
	{
        [SerializeField]
        protected UpgradeType type;
        public UpgradeType Type { get { return type; } } 

        [SerializeField]
        protected Data[] list;
        public Data[] List { get { return list; } }

        public int Max { get { return list.Length; } }

        public Data this[int index] { get { return list[index]; } }

        [Serializable]
        new public class Data
        {
            [SerializeField]
            protected Currency cost;
            public Currency Cost { get { return cost; } } 

            [SerializeField]
            protected float percentage;
            public float Percentage { get { return percentage; } } 
        }

        public uint Number { get; protected set; }

        public Data Current
        {
            get
            {
                if (Number == 0) return null;

                return list[Number - 1];
            }
        }
        public bool Maxed { get { return Current == list.Last(); } }

        public Data Next
        {
            get
            {
                if (Maxed) return null;

                return list[Number];
            }
        }
        public bool CanAfford
        {
            get
            {
                if (Maxed) return false;

                return Proponent.Funds.CanAfford(Next.Cost);
            }
        }

        public virtual float Sample(float value)
        {
            if (Current == null) return value;

            return value + value * Current.Percentage / 100;
        }

        public override void Configure(ProponentUpgrades data)
        {
            base.Configure(data);

            Number = 0;
        }

        public event Action OnUpgrade;
        public virtual void Upgrade()
        {
            if (Maxed)
                throw new Exception(GetType().Name + " already maxed out, can't add to it anymore");

            if (!CanAfford)
                throw new Exception(Proponent.name + " can't afford upgrade " + GetType().Name + " yet they are trying to upgrade");

            Proponent.Funds.Take(Next.Cost);
            Number++;

            if (OnUpgrade != null) OnUpgrade();
        }
    }
}