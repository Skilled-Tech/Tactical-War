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
        public PlayerUI HUD { get; protected set; }

        public Core Core { get { return Core.Instance; } }

        new public abstract class Module : Module<PlayerProponent>
        {
            public PlayerProponent Player { get { return Reference; } }
        }

        public override LevelCore.ProponentProperty LevelData => Level.Data.Level.Player;

        protected override void Awake()
        {
            base.Awake();

            HUD = FindObjectOfType<PlayerUI>();

            Modules.Configure(this);
            HUD.Configure(this);
        }

        protected override void Start()
        {
            base.Start();

            Modules.Init(this);
            HUD.Init();
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                Enemy.enabled = false;

            if (Input.GetKeyDown(KeyCode.W))
                Enemy.Base.Suicide();
        }
    }
}