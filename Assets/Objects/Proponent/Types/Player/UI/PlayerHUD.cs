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

using TMPro;

namespace Game
{
    public class PlayerHUD : PlayerProponent.Module
    {
        public PlayerHUDUnits Units { get; protected set; }

        public abstract class Module : UIElement, IModule<PlayerHUD>
        {
            public PlayerHUD HUD { get; protected set; }

            public PlayerProponent Proponent { get { return HUD.Player; } }

            public Core Core { get { return Core.Instance; } }
            public PlayerCore Player { get { return Core.Player; } }

            public virtual void Configure(PlayerHUD data)
            {
                HUD = data;
            }

            public virtual void Init()
            {

            }
        }

        public Level Level { get { return Level.Instance; } }
        public LevelProponents Proponents { get { return Level.Proponents; } }
        public Proponent Enemy { get { return Proponents.Proponent2; } }

        public override void Configure(PlayerProponent data)
        {
            base.Configure(data);

            Units = Dependancy.Get<PlayerHUDUnits>(gameObject);

            Modules.Configure(this);
        }

        public override void Init()
        {
            base.Init();

            Units.Set(Player.Base.Units);

            Modules.Init(this);
        }
    }
}