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
    public class BaseTowerSlotTurret : Base.Module
    {
        [SerializeField]
        protected Currency cost;
        public Currency Cost { get { return cost; } }

        [SerializeField]
        protected Turret instance;
        public Turret Instance { get { return instance; } }

        public bool isDeployed
        {
            get
            {
                return instance.gameObject.activeSelf;
            }
            set
            {
                instance.gameObject.SetActive(value);
            }
        }

        public BaseTowerSlot Slot { get; protected set; }
        public virtual void Set(BaseTowerSlot slot)
        {
            this.Slot = slot;
        }

        public override void Configure(Base data)
        {
            base.Configure(data);

            isDeployed = false;
        }

        public override void Init()
        {
            base.Init();

            instance.Init(Slot);
        }
    }
}