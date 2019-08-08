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
    public class BaseTowerSlots : Base.Module
    {
        [SerializeField]
        protected BaseTowerSlot[] list;
        public BaseTowerSlot[] List { get { return list; } }

        public BaseTowerSlot this[int index]
        {
            get
            {
                return list[index];
            }
        }

        [SerializeField]
        protected BaseTowerSlotContextUI contextUI;
        public BaseTowerSlotContextUI ContextUI { get { return contextUI; } }

        public BaseTower Tower { get { return Base.Tower; } }

        public override void Configure(Base data)
        {
            base.Configure(data);

            contextUI.Configure(this);
        }

        public override void Init()
        {
            base.Init();

            foreach (var slot in list)
            {
                slot.Handle.SetActive(slot.Index == 0);

                slot.OnDeploy += () => OnSlotDeployed(slot);
            }

            contextUI.Init();
        }

        void OnSlotDeployed(BaseTowerSlot slot)
        {
            if(slot.Index < list.Length - 1 && Proponent is PlayerProponent)
            {
                list[slot.Index + 1].Handle.SetActive(true);
            }
        }
    }
}