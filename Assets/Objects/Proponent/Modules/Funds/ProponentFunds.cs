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
	public class ProponentFunds : Proponent.Module
    {
		public ProponentGoldFunds Gold { get; protected set; }
		public ProponentXPFunds XP { get; protected set; }

        public event Action OnValueChanged;

        public abstract class Reference : Module<ProponentFunds>
        {
            public ProponentFunds Funds { get { return Data; } }
        }
        public abstract class Module : Reference
        {
            [SerializeField]
            protected int _value;
            public int Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    if (value < 0) value = 0;

                    _value = value;

                    if (OnValueChanged != null) OnValueChanged(Value);
                }
            }

            public event Action<float> OnValueChanged;
        }

        public override void Configure(Proponent data)
        {
            base.Configure(data);

            Gold = Dependancy.Get<ProponentGoldFunds>(Proponent.gameObject);
            Gold.OnValueChanged += OnGoldChanged;

            XP = Dependancy.Get<ProponentXPFunds>(Proponent.gameObject);
            XP.OnValueChanged += OnXPChanged;

            Modules.Configure(this);
        }

        public override void Init()
        {
            base.Init();

            Modules.Init(this);
        }

        public bool CanAfford(Currency cost)
        {
            return Currency.IsSufficient(cost, Gold.Value, XP.Value);
        }

        public virtual void Take(Currency value)
        {
            if(CanAfford(value))
            {
                Gold.Value -= value.Gold;

                XP.Value -= value.XP;
            }
            else
            {
                throw new Exception("Trying to reduce " + value.ToString() + " From " + Proponent.name + " Funds but said Proponent doesn't have enough funds");
            }
        }

        void OnGoldChanged(float value)
        {
            if (OnValueChanged != null) OnValueChanged();
        }
        void OnXPChanged(float value)
        {
            if (OnValueChanged != null) OnValueChanged();
        }
    }
}