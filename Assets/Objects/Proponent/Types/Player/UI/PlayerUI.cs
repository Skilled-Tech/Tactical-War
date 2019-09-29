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
    public class PlayerUI : PlayerProponent.Module
    {
        public ProponentUnitsUI Units { get; protected set; }

        public abstract class Module : UIElement, IModule<PlayerUI>
        {
            public PlayerUI HUD { get; protected set; }

            public PlayerProponent Proponent { get { return HUD.Player; } }

            public Base Base { get { return Proponent.Base; } }

            public Core Core { get { return Core.Instance; } }
            public PlayerCore Player { get { return Core.Player; } }

            public virtual void Configure(PlayerUI data)
            {
                HUD = data;
            }

            public virtual void Init()
            {

            }
        }

        public Level Level { get { return Level.Instance; } }
        public LevelProponents Proponents { get { return Level.Proponents; } }
        public Proponent Enemy { get { return Proponents.AI; } }

        public override void Configure(PlayerProponent data)
        {
            base.Configure(data);

            Units = Dependancy.Get<ProponentUnitsUI>(gameObject);

            Modules.Configure(this);
        }

        public override void Init()
        {
            base.Init();

            Modules.Init(this);
        }
    }
}