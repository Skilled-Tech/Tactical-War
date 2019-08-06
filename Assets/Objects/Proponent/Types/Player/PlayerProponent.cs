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
	public class PlayerProponent : Proponent
	{
		public PlayerHUD HUD { get; protected set; }

        new public abstract class Module : Module<PlayerProponent>
        {
            public PlayerProponent Player { get { return Data; } }
        }

        protected override void Awake()
        {
            base.Awake();

            HUD = FindObjectOfType<PlayerHUD>();

            Modules.Configure(this);
            HUD.Configure(this);
        }

        protected override void Start()
        {
            base.Start();

            Modules.Init(this);
            HUD.Init();
        }
    }
}