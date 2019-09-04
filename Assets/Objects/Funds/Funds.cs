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

using PlayFab;

namespace Game
{
    [Serializable]
	public class Funds
    {
        [SerializeField]
        protected GoldProperty gold;
        public GoldProperty Gold { get { return gold; } }
        public class GoldProperty : Property
        {
            public override string Code => "GD";

            public GoldProperty(int value) : base(value)
            {

            }
        }

        [SerializeField]
        protected JewelsProperty jewels;
        public JewelsProperty Jewels { get { return jewels; } }
        public class JewelsProperty : Property
        {
            public override string Code => "JL";

            public JewelsProperty(int value) : base(value)
            {

            }
        }

        public event Action OnValueChanged;

        public virtual Property Get(CurrencyType type)
        {
            switch (type)
            {
                case CurrencyType.Gold:
                    return Gold;
                case CurrencyType.Jewels:
                    return Jewels;
            }

            throw new NotImplementedException();
        }

        public virtual void Load(Dictionary<string, int> dictionary)
        {
            gold.Value = dictionary[gold.Code];

            jewels.Value = dictionary[jewels.Code];
        }

        [Serializable]
        public abstract class Property
        {
            public abstract string Code { get; }

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

            public Property(int value)
            {
                this.Value = value;
            }
        }

        public virtual void Configure(int value)
        {
            this.gold.Value = value;
            this.jewels.Value = value;

            Configure();
        }
        public virtual void Configure()
        {
            Gold.OnValueChanged += OnGoldChanged;

            Jewels.OnValueChanged += OnJewelsChanged;
        }

        public bool CanAfford(Currency cost)
        {
            return Currency.IsSufficient(cost, Gold.Value, Jewels.Value);
        }

        public virtual void Take(Currency value)
        {
            if(CanAfford(value))
            {
                Gold.Value -= value.Gold;

                Jewels.Value -= value.Jewels;
            }
            else
            {
                throw new Exception("Trying to reduce " + value.ToString() + " From " + ToString() + " but the funds are too low, man");
            }
        }
        public virtual void Add(Currency value)
        {
            Gold.Value += value.Gold;

            Jewels.Value += value.Jewels;
        }

        void OnGoldChanged(float value)
        {
            if (OnValueChanged != null) OnValueChanged();
        }
        void OnJewelsChanged(float value)
        {
            if (OnValueChanged != null) OnValueChanged();
        }

        public override string ToString()
        {
            return Currency.FormatText(gold.Value, jewels.Value);
        }

        public Funds(int value)
        {
            gold = new GoldProperty(value);

            jewels = new JewelsProperty(value);
        }
    }
}