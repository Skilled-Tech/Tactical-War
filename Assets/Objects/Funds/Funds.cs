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
        #region  Elements
        [SerializeField]
        private ElementData[] elements;
        public ElementData[] Elements { get { return elements; } }
        [Serializable]
        public class ElementData
        {
            [SerializeField]
            private CurrencyType type;
            public CurrencyType Type { get { return type; } }

            [SerializeField]
            protected long _value;
            public long Value
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

            public event Action<long> OnValueChanged;

            public ElementData(CurrencyType type, long value)
            {
                this.type = type;
                this.Value = value;
            }
            public ElementData(string typeCode, long value) : this(CurrencyCode.To(typeCode), value)
            {

            }
            public ElementData(KeyValuePair<string, long> pair) : this(pair.Key, pair.Value)
            {

            }
        }

        public event Action OnValueChanged;

        public virtual ElementData Find(CurrencyType type)
        {
            for (int i = 0; i < elements.Length; i++)
                if (elements[i].Type == type)
                    return elements[i];

            throw new NotImplementedException();
        }
        #endregion

        public virtual void Configure(long value)
        {
            Configure();
        }
        public virtual void Configure()
        {
            for (int i = 0; i < elements.Length; i++)
            {
                var element = elements[i];

                element.OnValueChanged += (long value) => OnElementValueChanged(element, value);
            }
        }

        public bool CanAfford(Currency cost)
        {
            var element = Find(cost.Type);

            return element.Value >= cost.Value;
        }

        public virtual void Take(Currency currency)
        {
            var element = Find(currency.Type);

            if (CanAfford(currency))
            {
                element.Value -= currency.Value;
            }
            else
            {
                throw new Exception("Trying to reduce " + currency.ToString() + " From " + element.ToString() + " but the funds are too low, man");
            }
        }
        public virtual void Add(Currency currency)
        {
            var element = Find(currency.Type);

            element.Value += currency.Value;
        }

        public virtual void Load(Dictionary<string, int> dictionary)
        {
            foreach (var pair in dictionary)
            {
                var type = CurrencyCode.To(pair.Key);

                var element = Find(type);

                element.Value = pair.Value;
            }
        }

        void OnElementValueChanged(ElementData element, long value)
        {
            if (OnValueChanged != null) OnValueChanged();
        }

        public Funds(int value)
        {
            var types = Enum.GetValues(typeof(CurrencyType));

            elements = new ElementData[types.Length];

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = new ElementData((CurrencyType)types.GetValue(i), value);
            }
        }
    }
}