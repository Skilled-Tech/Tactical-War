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
	public class BaseTowerSlotContextUI : UIElement
	{
        public BaseTowerSlotBuyContextUI Buy { get; protected set; }
        public BaseTowerSlotsUseContextUI Use { get; protected set; }
        public BaseTowerSlotBuyContextUI Sell { get; protected set; }

        public abstract class Element : UIElement, IModule<BaseTowerSlotContextUI>
        {
            public BaseTowerSlotContextUI Context { get; protected set; }

            new public BaseTowerSlot Target { get { return Context.Target; } }

            public virtual void Configure(BaseTowerSlotContextUI data)
            {
                this.Context = data;
            }

            public virtual void Init()
            {
                Context.OnTargetChanged += OnTargetChange;
            }

            protected virtual void OnTargetChange(BaseTowerSlot slot)
            {
                if (IsApplicaple(slot))
                    Show();
                else
                    Hide();
            }

            protected virtual void UpdateTarget()
            {
                Context.Show(Target);
            }

            protected abstract bool IsApplicaple(BaseTowerSlot slot);
        }

        BaseTowerSlots slots;
        public virtual void Configure(BaseTowerSlots slots)
        {
            this.slots = slots;

            Buy = Dependancy.Get<BaseTowerSlotBuyContextUI>(gameObject);
            Use = Dependancy.Get<BaseTowerSlotsUseContextUI>(gameObject);
            Sell = Dependancy.Get<BaseTowerSlotBuyContextUI>(gameObject);

            Modules.Configure(this);
        }

        public virtual void Init()
        {
            Modules.Init(this);
        }

        protected BaseTowerSlot _target;
        new public BaseTowerSlot Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;

                if (OnTargetChanged != null) OnTargetChanged(Target);
            }
        }
        public event Action<BaseTowerSlot> OnTargetChanged;

        public virtual void Show(BaseTowerSlot context)
        {
            this.Target = context;

            Show();
        }
    }
}