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
    [CreateAssetMenu]
	public class Funds : ScriptableObject
    {
        [SerializeField]
        protected Property gold;
        public Property Gold { get { return gold; } }

        [SerializeField]
        protected Property xp;
        public Property XP { get { return xp; } }

        public virtual Property Get(CurrencyType type)
        {
            switch (type)
            {
                case CurrencyType.Gold:
                    return Gold;
                case CurrencyType.XP:
                    return XP;
            }

            throw new NotImplementedException();
        }

        public event Action OnValueChanged;

        [Serializable]
        public class Property
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

        public virtual void Configure()
        {
            Gold.OnValueChanged += OnGoldChanged;

            XP.OnValueChanged += OnXPChanged;
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
                throw new Exception("Trying to reduce " + value.ToString() + " From " + name + " but the funds are too low, man");
            }
        }
        public virtual void Add(Currency value)
        {
            Gold.Value += value.Gold;

            XP.Value += value.XP;
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