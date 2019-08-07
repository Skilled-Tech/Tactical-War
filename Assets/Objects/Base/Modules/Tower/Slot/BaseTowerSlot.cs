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
	public class BaseTowerSlot : Base.Module
	{
		[SerializeField]
        protected Currency cost = new Currency(100, 50);
        public Currency Cost { get { return cost; } }

        [SerializeField]
        protected Turret turret;
        public Turret Turret { get { return turret; } }

        public override void Init()
        {
            base.Init();

            turret.Init(this);
        }
    }
}