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

using UnityEngine.EventSystems;

namespace Game
{
    public class BaseTowerSlot : Base.Module, IPointerClickHandler
    {
        [SerializeField]
        protected int cost = 100;
        public int Cost { get { return cost; } }

        [SerializeField]
        protected GameObject graphics;
        public GameObject Graphics { get { return graphics; } }

        [SerializeField]
        protected GameObject handle;
        public GameObject Handle { get { return handle; } }

        public bool isDeployed { get { return graphics.activeSelf; } }
        public event Action OnDeploy;
        public virtual void Deploy()
        {
            graphics.SetActive(true);

            handle.SetActive(false);

            if (OnDeploy != null) OnDeploy();
        }

        public int Index { get { return Array.IndexOf(Slots.List, this); } }

        public BaseTowerSlotTurret Turret { get; protected set; }
        public BaseTowerSlots Slots { get { return Base.Tower.Slots; } }

        public override void Configure(Base data)
        {
            base.Configure(data);

            Turret = Dependancy.Get<BaseTowerSlotTurret>(gameObject);
            Turret.Set(this);
        }

        public override void Init()
        {
            base.Init();
        }

        public event Action OnClick;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick != null) OnClick();

            Slots.ContextUI.Show(this);
        }
    }
}