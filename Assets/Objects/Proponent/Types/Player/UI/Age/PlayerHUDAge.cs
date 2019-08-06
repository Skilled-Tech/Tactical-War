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
	public class PlayerHUDAge : PlayerHUD.Reference
	{
        public PlayerHUDUnitCreator UnitCreator { get; protected set; }

        public abstract class Module : Module<PlayerHUDAge>
        {
            public PlayerHUDAge Age { get { return Data; } }
        }

        public override void Configure(PlayerHUD data)
        {
            base.Configure(data);

            UnitCreator = Dependancy.Get<PlayerHUDUnitCreator>(gameObject);

            Modules.Configure(this);
        }

        public override void Init()
        {
            base.Init();

            Modules.Init(this);
        }

        public virtual void Set(Age age)
        {
            UnitCreator.Set(age);
        }
    }
}